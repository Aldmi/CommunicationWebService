using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Base;
using Exchange.Base.Model;
using Exchange.MasterSerialPort.Option;
using Shared.Enums;
using Transport.SerialPort.Abstract;
using Worker.Background.Abstarct;


namespace Exchange.MasterSerialPort
{
    public abstract class BaseExchangeSerialPort : IExhangeBehavior
    {
        #region field

        private readonly ISerailPort _serailPort;
        private readonly IBackgroundService _backgroundService;
        protected readonly ExchangeMasterSpOption ExchangeMasterSpOption;

        #endregion




        #region prop

        public string TransportName => _serailPort.SerialOption.Port;
        public UniversalInputType LastSendData { get; }
        public IEnumerable<string> GetRuleNames => new List<string>(); //TODO: сейчас в ExchangeMasterSpOption только 1 ExchangeRule, должно быть список.
        public TypeExchange TypeExchange => TypeExchange.SerialPort;

        protected ConcurrentQueue<UniversalInputType> InDataQueue { get; set; } = new ConcurrentQueue<UniversalInputType>();
        protected UniversalInputType Data4CycleFunc { get; set; }

        #endregion




        #region ctor

        protected BaseExchangeSerialPort(ISerailPort serailPort, IBackgroundService backgroundService, ExchangeMasterSpOption exchangeMasterSpOption)
        {
            _serailPort = serailPort;
            _backgroundService = backgroundService;
            ExchangeMasterSpOption = exchangeMasterSpOption;

            StartCycleExchange(); //TODO: запускать ценрализованно как backGround
        }

        #endregion




        #region Methode

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





        #region abstractMember

        protected abstract Task OneTimeExchangeActionAsync(CancellationToken ct);
        protected abstract Task CycleTimeExchangeActionAsync(CancellationToken ct);

        #endregion
    }
}