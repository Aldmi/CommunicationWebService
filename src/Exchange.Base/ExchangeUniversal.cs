using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using Exchange.Base.RxModel;
using InputDataModel.Base;
using Serilog;
using Shared.Enums;
using Shared.Types;
using Transport.Base.Abstract;
using Transport.Base.RxModel;
using Worker.Background.Abstarct;


namespace Exchange.Base
{
    public class ExchangeUniversal<TIn> : IExchange<TIn>
    {
        #region field
        protected readonly ExchangeOption ExchangeOption;
        private readonly ITransport _transport;
        private readonly ITransportBackground _transportBackground;
        private readonly IExchangeDataProvider<TIn, ResponseDataItem<TIn>> _dataProvider;                               //проавйдер данных является StateFull, т.е. хранит свое последнее состояние между отправкой данных
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<InDataWrapper<TIn>> _inDataQueue  = new ConcurrentQueue<InDataWrapper<TIn>>(); //Очередь данных для SendOneTimeData().
        private InDataWrapper<TIn> _data4CycleFunc;                                                                     //Данные для Цикл. функции.
        #endregion



        #region prop
        public string KeyExchange => ExchangeOption.Key;
        public bool AutoStartCycleFunc => ExchangeOption.AutoStartCycleFunc;
        public KeyTransport KeyTransport => ExchangeOption.KeyTransport;
        public bool IsOpen => _transport.IsOpen;
        public bool IsStartedTransportBg => _transportBackground.IsStarted;
        public bool IsStartedCycleExchange { get; private set; }

        private bool _isConnect;
        public bool IsConnect
        {
            get => _isConnect;
            private set
            {
                if (value == _isConnect) return;
                _isConnect = value;
                IsConnectChangeRx.OnNext(new ConnectChangeRxModel { IsConnect = _isConnect, KeyExchange = KeyExchange});
            }
        }

        private InDataWrapper<TIn> _lastSendData;
        public InDataWrapper<TIn> LastSendData
        {
            get => _lastSendData;
            private set
            {
                if (value == _lastSendData) return;
                _lastSendData = value;
                LastSendDataChangeRx.OnNext(new LastSendDataChangeRxModel<TIn> {KeyExchange = KeyExchange, LastSendData = LastSendData});
            }
        }
        #endregion



        #region ctor

        public ExchangeUniversal(ExchangeOption exchangeOption,
                                 ITransport transport,
                                 ITransportBackground transportBackground,
                                 IExchangeDataProvider<TIn,
                                 ResponseDataItem<TIn>> dataProvider,
                                 ILogger logger)
        {
            ExchangeOption = exchangeOption;
            _transport = transport;
            _transportBackground = transportBackground;
            _dataProvider = dataProvider;
            _logger = logger;
        }

        #endregion



        #region ExchangeRx
        public ISubject<ConnectChangeRxModel> IsConnectChangeRx { get; } = new Subject<ConnectChangeRxModel>();
        public ISubject<LastSendDataChangeRxModel<TIn>> LastSendDataChangeRx { get; } = new Subject<LastSendDataChangeRxModel<TIn>>();
        public ISubject<ResponsePieceOfDataWrapper<TIn>> ResponseChangeRx { get; } = new Subject<ResponsePieceOfDataWrapper<TIn>>();
        #endregion



        #region TransportRx
        public ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx => _transport.IsOpenChangeRx;
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx => _transport.StatusDataExchangeChangeRx;
        public ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx => _transport.StatusStringChangeRx;
        #endregion



        #region Methode

        #region ReOpen

        /// <summary>
        /// Циклическое открытие подключения
        /// </summary>
        private CancellationTokenSource _cycleReOpenedCts;
        public async Task CycleReOpened()
        {
            _logger.Debug($"KeyExchange {KeyExchange} START>>>>>>>");//DEBUG
            if (_transport != null)
            {
                if (_transport.IsCycleReopened)
                {
                    _logger.Error($"ТРАНСПОРТ УЖЕ НАХОДИТСЯ В ЦИКЛЕ ПЕРЕОТКРЫТИЯ \"{_transport.KeyTransport}\"  KeyExchange \"{KeyExchange}\"");
                    return;
                }
                _cycleReOpenedCts?.Cancel();
                _cycleReOpenedCts = new CancellationTokenSource();
                await Task.Factory.StartNew(async () =>
                {
                    await _transport.CycleReOpened();
                }, _cycleReOpenedCts.Token);
            }
            _logger.Debug($"KeyExchange {KeyExchange} STOP<<<<<<<<");//DEBUG
        }

        /// <summary>
        /// Отмена задачи циклического открытия подключения
        /// </summary>
        public void CycleReOpenedCancelation()
        {
            if (!IsOpen)
            {
                _transport.CycleReOpenedCancelation();
                _cycleReOpenedCts?.Cancel();
            }
        }

        #endregion


        #region CycleExchange

        /// <summary>
        /// Добавление ЦИКЛ. функций на БГ
        /// </summary>
        public void StartCycleExchange()
        {
            _transportBackground.AddCycleAction(CycleTimeActionAsync);
            IsStartedCycleExchange = true;
        }

        /// <summary>
        /// Удаление ЦИКЛ. функций из БГ
        /// </summary>
        public void StopCycleExchange()
        {
            _transportBackground.RemoveCycleFunc(CycleTimeActionAsync);
            IsStartedCycleExchange = false;
        }

        #endregion


        #region SendData

        /// <summary>
        /// Отправить команду. аналог однократно выставляемой функции.
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(Command4Device command)
        {
            if (command != Command4Device.None)
            {
                var dataWrapper = new InDataWrapper<TIn> { Command = command};
                _inDataQueue.Enqueue(dataWrapper);
                _transportBackground.AddOneTimeAction(OneTimeActionAsync);
            }
        }


        /// <summary>
        /// Отправить данные для однократно выставляемой функции.
        /// Функция выставляется на БГ.
        /// </summary>
        public void SendOneTimeData(IEnumerable<TIn> inData)
        {
            if (inData != null)
            {
                var dataWrapper= new InDataWrapper<TIn>{Datas = inData.ToList()};
                _inDataQueue.Enqueue(dataWrapper);
                _transportBackground.AddOneTimeAction(OneTimeActionAsync);
            }
        }


        /// <summary>
        /// Выставить данные для цикл. функции.
        /// </summary>
        public void SendCycleTimeData(IEnumerable<TIn> inData)
        {
            //TODO: В текущей реализации Цикл. Отправки данных, данные переданные в inData могут пропасть.
            //TODO: Т.к. сохраняется только одно (последнее занчение) для отправки.
            //TODO: Если на медленном трансопрте чато менять данные для цикл отпр, то будет потеря данных.
            //TODO: Можно сделать Очередь отправки данных (только данных, не команд как в OneTimeActionAsync)
            //TODO: В SendCycleTimeData данные не перезаписывают одну переменную _data4CycleFunc, а копяться в очереди.
            //TODO: В CycleTimeActionAsync разматывается очередь по принципу 
            //TODO: "если элементов > 1, то TryDequeue элемент из очереди и отправляем его"
            //TODO: "если элементов == 1 элемента, то TryPeek элемент из очереди и отправляем его"
            //TODO: В устоявщемся режиме работы должен быть всегда 1 элемент.
            //TODO: Вести контроль переполнения очереди ("если элементов > 100, то элементы не добавлять и сообщть от этом в возвращаемом занчении SendCycleTimeData").
            if (inData != null)
            {
                var dataWrapper = new InDataWrapper<TIn> { Datas = inData.ToList() };
                _data4CycleFunc = dataWrapper;
            }
        }

        #endregion


        #region Actions

        /// <summary>
        /// Однократно вызываемая функция.
        /// </summary>
        protected async Task OneTimeActionAsync(CancellationToken ct)
        {
            if (_inDataQueue.TryDequeue(out var inData))
            {
                var transportResponseWrapper = await SendingPieceOfData(inData, ct);
                transportResponseWrapper.KeyExchange = KeyExchange;
                transportResponseWrapper.DataAction = (inData.Command == Command4Device.None) ? DataAction.OneTimeAction : DataAction.CommandAction;
                ResponseChangeRx.OnNext(transportResponseWrapper);
            }
        }

        /// <summary>
        /// Обработка отправки цикл. даных.
        /// </summary>
        protected async Task CycleTimeActionAsync(CancellationToken ct)
        {
            var inData = _data4CycleFunc;
            var transportResponseWrapper = await SendingPieceOfData(inData, ct);
            transportResponseWrapper.KeyExchange = KeyExchange;
            transportResponseWrapper.DataAction = DataAction.CycleAction;
            ResponseChangeRx.OnNext(transportResponseWrapper);
            await Task.Delay(3000, ct); //DEBUG
        }

        /// <summary>
        /// Отправка порции данных.
        /// </summary>
        /// <returns>Ответ на отправку порции данных</returns>
        private int _countTryingTakeData = 0;
        private async Task<ResponsePieceOfDataWrapper<TIn>> SendingPieceOfData(InDataWrapper<TIn> inData, CancellationToken ct)
        {
            var transportResponseWrapper = new ResponsePieceOfDataWrapper<TIn>();
            //ПОДПИСКА НА СОБЫТИЕ ОТПРАВКИ ПОРЦИИ ДАННЫХ
            var subscription = _dataProvider.RaiseSendDataRx.Subscribe(provider =>
            {
                var transportResp = new ResponseDataItem<TIn>();
                var status = StatusDataExchange.None;
                try
                {
                    status = _transport.DataExchangeAsync(_dataProvider.TimeRespone, provider, ct).GetAwaiter().GetResult();
                    switch (status)
                    {
                        //ОБМЕН ЗАВЕРШЕН ПРАВИЛЬНО.
                        case StatusDataExchange.End:
                            IsConnect = true;
                            _countTryingTakeData = 0;
                            LastSendData = provider.InputData;
                            transportResp.ResponseData = provider.OutputData.ResponseData;
                            transportResp.Encoding = provider.OutputData.Encoding;
                            transportResp.IsOutDataValid = provider.OutputData.IsOutDataValid;
                            break;

                        //ТАЙМАУТ ОТВЕТА.
                        case StatusDataExchange.EndWithTimeout: //TODO: Считать ли кол-во таймаутов до переоткрытия?
                            IsConnect = false;
                            break;

                        //ОБМЕН ЗАВЕРЩЕН КРИТИЧЕСКИ НЕ ПРАВИЛЬНО. ПЕРЕОТКРЫТИЕ СОЕДИНЕНИЯ.
                        case StatusDataExchange.EndWithTimeoutCritical:
                        case StatusDataExchange.EndWithErrorCritical:
                            CycleReOpened(); 
                            _logger.Warning($"ОБМЕН ЗАВЕРЩЕН КРИТИЧЕСКИ НЕ ПРАВИЛЬНО. ПЕРЕОТКРЫТИЕ СОЕДИНЕНИЯ. KeyExchange {KeyExchange}");
                            break;

                        //ОБМЕН ЗАВЕРШЕН НЕ ПРАВИЛЬНО.
                        default:
                            if (++_countTryingTakeData > ExchangeOption.CountBadTrying)
                            {
                                _countTryingTakeData = 0;
                                IsConnect = false;
                                _logger.Warning($"ОБМЕН ЗАВЕРШЕН НЕ ПРАВИЛЬНО. KeyExchange {KeyExchange}");
                                CycleReOpened();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    //ОШИБКА ТРАНСПОРТА.
                    IsConnect = false;
                    transportResp.TransportException = ex;
                    _logger.Error(ex, $"ОШИБКА ТРАНСПОРТА. KeyExchange {KeyExchange}");
                }
                finally
                {
                    transportResp.RequestData = provider.InputData;
                    transportResp.Status = status;
                    transportResp.Message = provider.StatusString.ToString();
                    transportResponseWrapper.ResponsesItems.Add(transportResp);
                }
            });

            int countTryingSendData = 0;
            try
            {   //ЗАПУСК КОНВЕЕРА ПОДГОТОВКИ ДАННЫХ К ОБМЕНУ
                countTryingSendData= await _dataProvider.StartExchangePipeline(inData);
            }
            catch (Exception ex)
            {
                //ОШИБКА ПОДГОТОВКИ ДАННЫХ К ОБМЕНУ.
                IsConnect = false;
                transportResponseWrapper.ExceptionExchangePipline = ex;
                transportResponseWrapper.Message = _dataProvider.StatusString.ToString();
                _logger.Error(ex, $"ОШИБКА ПОДГОТОВКИ ДАННЫХ К ОБМЕНУ. KeyExchange {KeyExchange}");
            }
            finally
            {
                subscription.Dispose();      
            }

            var countIsValid = transportResponseWrapper.ResponsesItems.Count(resp => resp.IsOutDataValid);
            var countAll = transportResponseWrapper.ResponsesItems.Count(resp => resp.IsOutDataValid);
            _logger.Information($"ОТВЕТ НА ПАКЕТНУЮ ОТПРАВКУ ПОЛУЧЕН . KeyExchange= {KeyExchange} успех/ответов/запросов=  ({countIsValid} / {countAll} / {countTryingSendData})");
          
            return transportResponseWrapper;
        }

        #endregion

        #endregion



        #region Disposable

        public void Dispose()
        {

        }

        #endregion
    }
}