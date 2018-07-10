using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Base;
using Exchange.Base.Model;
using Exchange.MasterSerialPort.Option;
using Shared.Types;
using Transport.Base.RxModel;
using Transport.SerialPort.Abstract;
using Worker.Background.Abstarct;


namespace Exchange.MasterSerialPort
{
    public abstract class BaseExchangeSerialPort : IExchange
    {
        #region field

        private readonly ISerailPort _serailPort;
        private readonly IBackgroundService _backgroundService;
        protected readonly ExchangeMasterSpOption ExchangeMasterSpOption;

        #endregion




        #region prop

        public bool IsOpen => _serailPort.IsOpen;
        public bool IsConnect { get; }

        public UniversalInputType LastSendData { get; }
        public KeyExchange GetKeyExchange => _backgroundService.KeyExchange;

        public IEnumerable<string> GetRuleNames => new List<string>(); //TODO: сейчас в ExchangeMasterSpOption только 1 ExchangeRule, должно быть список.
     
        protected ConcurrentQueue<UniversalInputType> InDataQueue { get; set; } = new ConcurrentQueue<UniversalInputType>();
        protected UniversalInputType Data4CycleFunc { get; set; }

        #endregion




        #region ctor

        protected BaseExchangeSerialPort(ISerailPort serailPort, IBackgroundService backgroundService, ExchangeMasterSpOption exchangeMasterSpOption)
        {
            _serailPort = serailPort;
            _backgroundService = backgroundService;
            ExchangeMasterSpOption = exchangeMasterSpOption;
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
            _backgroundService.AddCycleAction(CycleTimeExchangeActionAsync);
        }


        /// <summary>
        /// 
        /// </summary>
        public void StopCycleExchange()
        {
            _backgroundService.RemoveCycleFunc(CycleTimeExchangeActionAsync);
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
                _backgroundService.AddOneTimeAction(OneTimeExchangeActionAsync);
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

        public ISubject<IExchange> IsDataExchangeSuccessChange { get; } //TODO: Добавить событие обмена
        public ISubject<IExchange> IsConnectChange { get; }
        public ISubject<IExchange> LastSendDataChange { get; }

        public ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx => _serailPort.IsOpenChangeRx;
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx => _serailPort.StatusDataExchangeChangeRx;
        public ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx => _serailPort.StatusStringChangeRx;

        #endregion
    }
}