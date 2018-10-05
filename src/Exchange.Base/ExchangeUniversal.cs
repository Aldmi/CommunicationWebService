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
using InputDataModel.Base;
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
        private readonly IExchangeDataProvider<TIn, ResponseDataItem<TIn>> _dataProvider;   //проавйдер данных является StateFull, т.е. хранит свое последнее состояние между отправкой данных
        #endregion



        #region prop

        public string KeyExchange => ExchangeOption.Key;
        public bool AutoStartCycleFunc => ExchangeOption.AutoStartCycleFunc;
        public KeyTransport KeyTransport => ExchangeOption.KeyTransport;
        public bool IsOpen => _transport.IsOpen;
        public bool IsStartedTransportBg => _transportBackground.IsStarted;
        public bool IsConnect { get; }
        public InDataWrapper<TIn> LastSendData { get; private set; }
        public bool IsStartedCycleExchange { get; set; }
        protected ConcurrentQueue<InDataWrapper<TIn>> InDataQueue { get; set; } = new ConcurrentQueue<InDataWrapper<TIn>>(); //Очередь данных для SendOneTimeData().
        protected InDataWrapper<TIn> Data4CycleFunc { get; set; }                                                            //Данные для Цикл. функции.

        #endregion



        #region ctor

        public ExchangeUniversal(ExchangeOption exchangeOption, ITransport transport, ITransportBackground transportBackground, IExchangeDataProvider<TIn, ResponseDataItem<TIn>> dataProvider)
        {
            ExchangeOption = exchangeOption;
            _transport = transport;
            _transportBackground = transportBackground;
            _dataProvider = dataProvider;

        }

        #endregion



        #region RxEvent

        public ISubject<IExchange<TIn>> IsDataExchangeSuccessChangeRx { get; } //TODO: Добавить событие обмена
        public ISubject<IExchange<TIn>> IsConnectChangeRx { get; }
        public ISubject<IExchange<TIn>> LastSendDataChangeRx { get; }

        public ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx => _transport.IsOpenChangeRx;
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx => _transport.StatusDataExchangeChangeRx;
        public ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx => _transport.StatusStringChangeRx;

        ISubject<IExchange<TIn>> IExchange<TIn>.IsDataExchangeSuccessChangeRx => throw new NotImplementedException();

        ISubject<IExchange<TIn>> IExchange<TIn>.IsConnectChangeRx => throw new NotImplementedException();

        ISubject<IExchange<TIn>> IExchange<TIn>.LastSendDataChangeRx => throw new NotImplementedException();

        public ISubject<OutResponseDataWrapper<TIn>> TransportResponseChangeRx { get; } = new Subject<OutResponseDataWrapper<TIn>>();
        

        #endregion




        #region Methode

        #region ReOpen

        /// <summary>
        /// Циклическое открытие подключения
        /// </summary>
        private CancellationTokenSource _cycleReOpenedCts;
        public async Task CycleReOpened()
        {
            if (_transport != null)
            {
                _cycleReOpenedCts?.Cancel();
                _cycleReOpenedCts = new CancellationTokenSource();
                await Task.Factory.StartNew(async () =>
                {
                    await _transport.CycleReOpened();
                }, _cycleReOpenedCts.Token);
            }
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
                InDataQueue.Enqueue(dataWrapper);
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
                InDataQueue.Enqueue(dataWrapper);
                _transportBackground.AddOneTimeAction(OneTimeActionAsync);
            }
        }


        /// <summary>
        /// Выставить данные для цикл. функции.
        /// </summary>
        public void SendCycleTimeData(IEnumerable<TIn> inData)
        {
            if (inData != null)
            {
                var dataWrapper = new InDataWrapper<TIn> { Datas = inData.ToList() };
                Data4CycleFunc = dataWrapper;
            }
        }

        #endregion


        #region Actions

        /// <summary>
        /// Однократно вызываемая функция.
        /// </summary>
        protected async Task OneTimeActionAsync(CancellationToken ct)
        {    
            if (InDataQueue.TryDequeue(out var inData))
            {
                var transportResponseWrapper = await SendingPieceOfData(inData, ct);
                transportResponseWrapper.KeyExchange = KeyExchange;
                transportResponseWrapper.DataAction = (inData.Command == Command4Device.None) ? DataAction.OneTimeAction : DataAction.CommandAction;
                TransportResponseChangeRx.OnNext(transportResponseWrapper);
            }
        }


        /// <summary>
        /// Обработка отправки цикл. даных.
        /// </summary>
        protected async Task CycleTimeActionAsync(CancellationToken ct)
        {
            var inData = Data4CycleFunc;
            var transportResponseWrapper = await SendingPieceOfData(inData, ct);
            transportResponseWrapper.KeyExchange = KeyExchange;
            transportResponseWrapper.DataAction = DataAction.CycleAction;
            TransportResponseChangeRx.OnNext(transportResponseWrapper);
        }



        /// <summary>
        /// Отправка порции данных
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task<OutResponseDataWrapper<TIn>> SendingPieceOfData(InDataWrapper<TIn> inData, CancellationToken ct)
        {
            var transportResponseWrapper = new OutResponseDataWrapper<TIn>();
            //ПОДПИСКА НА СОБЫТИЕ ОТПРАВКИ ПОРЦИИ ДАННЫХ
            var subscription = _dataProvider.RaiseSendDataRx.Subscribe(provider =>
            {
                var transportResponse = new ResponseDataItem<TIn>();
                var status = StatusDataExchange.None;
                try
                {
                    status = _transport.DataExchangeAsync(_dataProvider.TimeRespone, provider, ct).GetAwaiter().GetResult();
                    if (status == StatusDataExchange.End)
                    {
                        //ОТВЕТ ПОЛУЧЕНН.
                        LastSendData = provider.InputData;
                        transportResponse.ResponseData = provider.OutputData.ResponseData;
                        transportResponse.Encoding = provider.OutputData.Encoding;
                        transportResponse.IsOutDataValid = provider.OutputData.IsOutDataValid;
                    }
                }
                catch (Exception ex)
                {
                    //ОШИБКА ТРАНСПОРТА.
                    transportResponse.TransportException = ex;
                    Console.WriteLine(ex);
                }
                finally
                {
                    transportResponse.RequestData = provider.InputData;
                    transportResponse.Status = status;
                    transportResponse.Message = provider.StatusString.ToString();
                    transportResponseWrapper.ResponsesItems.Add(transportResponse);
                }
            });

            try
            {   //ЗАПУСК КОНВЕЕРА ПОДГОТОВКИ ДАННЫХ К ОБМЕНУ
                await _dataProvider.StartExchangePipeline(inData);
            }
            catch (Exception ex)
            {
                //ОШИБКА ПОДГОТОВКИ ДАННЫХ К ОБМЕНУ.
                transportResponseWrapper.ExceptionExchangePipline = ex;
                transportResponseWrapper.Message = _dataProvider.StatusString.ToString();
            }
            finally
            {
                //ОТПРАВИТЬ Rx СОБЫТИЕ С ОТВЕТОМ ОБ ОКОНЧАНИИ ОТПРАВКИ.
                subscription.Dispose();      
            }
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