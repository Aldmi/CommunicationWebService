using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange;
using Exchange.Base;
using Exchange.Base.Model;
using Shared.Types;
using Transport.Base.RxModel;
using Transport.Http.Abstract;
using Transport.Http.Concrete;
using Worker.Background.Abstarct;

namespace Exchange.Http
{
    public class BaseExchangeHttp : IExchange
    {
        #region field

        private readonly IHttp _httpTransport;
        private readonly IBackground _background;
        protected readonly ExchangeOption ExchangeOption;

        #endregion




        #region ctor
        //protected
        public BaseExchangeHttp(IHttp httpTransport, IBackground background, ExchangeOption exchangeOption) 
        {
            _httpTransport = httpTransport;
            _background = background;
            ExchangeOption = exchangeOption;
        }

        #endregion



        public string KeyExchange => ExchangeOption.Key;
        public bool AutoStart => ExchangeOption.AutoStartCycleFunc;
        public KeyTransport KeyTransport => ExchangeOption.KeyTransport;
        public bool IsOpen { get; }
        public bool IsConnect { get; }
        public UniversalInputType LastSendData { get; }
        public IEnumerable<string> GetRuleNames { get; }
        public bool IsStartedCycleExchange { get; set; }

        public Task CycleReOpened()
        {
            throw new System.NotImplementedException();
        }

        public void CycleReOpenedCancelation()
        {
            throw new System.NotImplementedException();
        }

        public void StartCycleExchange()
        {
            throw new System.NotImplementedException();
        }

        public void StopCycleExchange()
        {
            throw new System.NotImplementedException();
        }

        public void SendCommand(string commandName, UniversalInputType data4Command = null)
        {
            throw new System.NotImplementedException();
        }

        public void SendOneTimeData(UniversalInputType inData)
        {
            throw new System.NotImplementedException();
        }

        public void SendCycleTimeData(UniversalInputType inData)
        {
            throw new System.NotImplementedException();
        }

        public ISubject<IExchange> IsDataExchangeSuccessChangeRx { get; }
        public ISubject<IExchange> IsConnectChangeRx { get; }
        public ISubject<IExchange> LastSendDataChangeRx { get; }
        public ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx { get; }
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx { get; }
        public ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx { get; }



        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}