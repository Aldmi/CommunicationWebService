using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange;
using Exchange.Base.Model;
using Shared.Types;
using Transport.Base.Abstract;
using Transport.Base.DataProviderAbstract;
using Transport.Base.RxModel;
using Worker.Background.Abstarct;


namespace Exchange.Base
{
    public class Exchange<TOutput>  : IExchange
    {
        #region field

        private readonly ITransport _transport;
        private readonly IBackground _background;
        private readonly IExchangeDataProvider<UniversalInputType, TOutput> _dataProvider;
        protected readonly ExchangeOption ExchangeOption;

        #endregion



        #region prop

 
        public string KeyExchange => ExchangeOption.Key;
        public bool AutoStart => ExchangeOption.AutoStartCycleFunc;
        public KeyTransport KeyTransport => ExchangeOption.KeyTransport;
        public bool IsOpen => _transport.IsOpen;
        public bool IsConnect { get; }
        public UniversalInputType LastSendData { get; private set; }
        public IEnumerable<string> GetRuleNames => new List<string>(); //TODO: сейчас в ExchangeMasterSpOption только 1 ExchangeRule, должно быть список.
        public bool IsStartedCycleExchange { get; set; }
        protected ConcurrentQueue<UniversalInputType> InDataQueue { get; set; } = new ConcurrentQueue<UniversalInputType>(); //Очередь данных для SendOneTimeData().
        protected UniversalInputType Data4CycleFunc { get; set; }                                                           //Данные для Цикл. функции.

        #endregion




        #region ctor

        protected Exchange(ITransport transport, IBackground background, IExchangeDataProvider<UniversalInputType, TOutput> dataProvider, ExchangeOption exchangeOption)
        {
            _transport = transport;
            _background = background;
            _dataProvider = dataProvider;

            ExchangeOption = exchangeOption;
        }

        #endregion



        #region RxEvent

        public ISubject<IExchange> IsDataExchangeSuccessChangeRx { get; } //TODO: Добавить событие обмена
        public ISubject<IExchange> IsConnectChangeRx { get; }
        public ISubject<IExchange> LastSendDataChangeRx { get; }

        public ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx => _transport.IsOpenChangeRx;
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx => _transport.StatusDataExchangeChangeRx;
        public ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx => _transport.StatusStringChangeRx;

        #endregion




        #region Methode

        #region ReOpen

        /// <summary>
        /// Циклическое 
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
                    if (await _transport.CycleReOpened())
                    {
                        StartCycleExchange();
                    }
                }, _cycleReOpenedCts.Token);
            }
        }


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
            _background.AddCycleAction(CycleTimeActionAsync);
            IsStartedCycleExchange = true;
        }


        /// <summary>
        /// Удаление ЦИКЛ. функций из БГ
        /// </summary>
        public void StopCycleExchange()
        {
            _background.RemoveCycleFunc(CycleTimeActionAsync);
            IsStartedCycleExchange = false;
        }

        #endregion


        #region SendData

        /// <summary>
        /// Отправить команду. аналог однократно выставляемой функции.
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="data4Command"></param>
        public void SendCommand(string commandName, UniversalInputType data4Command = null)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Отправить данные для однократно выставляемой функции.
        /// Функция выставляется на БГ.
        /// </summary>
        public void SendOneTimeData(UniversalInputType inData)
        {
            if (inData != null)
            {
                InDataQueue.Enqueue(inData);
                _background.AddOneTimeAction(OneTimeActionAsync);
            }
        }


        /// <summary>
        /// Выставить данные для цикл. функции.
        /// </summary>
        public void SendCycleTimeData(UniversalInputType inData)
        {
            if (inData != null)
            {
                Data4CycleFunc = inData;
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
               _dataProvider.InputData = inData;
               var timeRespone = 1000;
               var dataExchangeSuccess = await _transport.DataExchangeAsync(timeRespone, _dataProvider, ct);
               LastSendData = _dataProvider.InputData;
            }
            await Task.Delay(1000, ct);
        }


        /// <summary>
        /// Обработка отправки цикл. даных.
        /// </summary>
        protected async Task CycleTimeActionAsync(CancellationToken ct)
        {
            await Task.Delay(2000, ct);//DEBUG
            //Console.WriteLine($"ExchangeOption.KeyTransport.Key=  {ExchangeOption.KeyTransport.Key}");//DEBUG
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