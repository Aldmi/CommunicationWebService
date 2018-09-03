using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange;
using Exchange.Base;
using Exchange.Base.Model;
using Shared.Types;
using Transport.Base.Abstract;
using Transport.Base.RxModel;
using Worker.Background.Abstarct;


namespace Exchange.MasterSerialPort
{
    public abstract class BaseExchangeSerialPort : IExchange
    {
        #region field

        private readonly ITransport _serailPort;
        private readonly IBackground _background;
        protected readonly ExchangeOption ExchangeOption;

        #endregion



        #region prop

        public string KeyExchange => ExchangeOption.Key;
        public bool AutoStart => ExchangeOption.AutoStartCycleFunc;
        public KeyTransport KeyTransport => ExchangeOption.KeyTransport;
        public bool IsOpen => _serailPort.IsOpen;
        public bool IsConnect { get; }
        public UniversalInputType LastSendData { get; }
        public IEnumerable<string> GetRuleNames => new List<string>(); //TODO: сейчас в ExchangeMasterSpOption только 1 ExchangeRule, должно быть список.
        public bool IsStartedCycleExchange { get; set; }
        protected ConcurrentQueue<UniversalInputType> InDataQueue { get; set; } =new ConcurrentQueue<UniversalInputType>();
        protected UniversalInputType Data4CycleFunc { get; set; }

        #endregion



        #region ctor

        protected BaseExchangeSerialPort(ITransport serailPort, IBackground background, ExchangeOption exchangeOption)
        {
            _serailPort = serailPort;
            _background = background;
            ExchangeOption = exchangeOption;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Циклическое 
        /// </summary>
        private CancellationTokenSource _cycleReOpenedCts;

        public async Task CycleReOpened()
        {
            if (_serailPort != null)
            {
                _cycleReOpenedCts?.Cancel();
                _cycleReOpenedCts = new CancellationTokenSource();
                await Task.Factory.StartNew(async () =>
                {
                    if (await _serailPort.CycleReOpened())
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
                _serailPort.CycleReOpenedCancelation();
                _cycleReOpenedCts?.Cancel();
            }
        }


        /// <summary>
        /// Добавление ЦИКЛ. функций
        /// </summary>
        public void StartCycleExchange()
        {
            _background.AddCycleAction(CycleTimeExchangeActionAsync);
            IsStartedCycleExchange = true;
        }


        /// <summary>
        /// 
        /// </summary>
        public void StopCycleExchange()
        {
            _background.RemoveCycleFunc(CycleTimeExchangeActionAsync);
            IsStartedCycleExchange = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="data4Command"></param>
        public void SendCommand(string commandName, UniversalInputType data4Command = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inData"></param>
        public void SendOneTimeData(UniversalInputType inData)
        {
            if (inData != null)
            {
                InDataQueue.Enqueue(inData);
                _background.AddOneTimeAction(OneTimeExchangeActionAsync);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inData"></param>
        public void SendCycleTimeData(UniversalInputType inData)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region AbstractMember

        protected abstract Task OneTimeExchangeActionAsync(CancellationToken ct);
        protected abstract Task CycleTimeExchangeActionAsync(CancellationToken ct);

        #endregion


        #region RxEvent

        public ISubject<IExchange> IsDataExchangeSuccessChangeRx { get; } //TODO: Добавить событие обмена
        public ISubject<IExchange> IsConnectChangeRx { get; }
        public ISubject<IExchange> LastSendDataChangeRx { get; }

        public ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx => _serailPort.IsOpenChangeRx;

        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx =>
            _serailPort.StatusDataExchangeChangeRx;

        public ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx => _serailPort.StatusStringChangeRx;

        #endregion


        #region Disposable

        public void Dispose()
        {
        }

        #endregion
    }
}