using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Enums;
using Shared.Types;
using Transport.Base.DataProvidert;
using Transport.Base.RxModel;
using Transport.TcpIp.Abstract;

namespace Transport.TcpIp.Concrete
{
    public class TcpIpTransport : ITcpIp
    {
        #region prop

        public TcpIpOption Option { get; }
        public KeyTransport KeyTransport { get; }

        #endregion




        #region ctor

        public TcpIpTransport(TcpIpOption option,  KeyTransport keyTransport)
        {
            Option = option;
            KeyTransport = keyTransport;
        }

        #endregion






        #region Disposable

        public void Dispose()
        {
            
        }

        #endregion

        public bool IsOpen { get; }
        public string StatusString { get; }
        public StatusDataExchange StatusDataExchange { get; }
        public bool IsCycleReopened { get; }
        public ISubject<IsOpenChangeRxModel> IsOpenChangeRx { get; }
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeRx { get; }
        public ISubject<StatusStringChangeRxModel> StatusStringChangeRx { get; }
        public Task<bool> CycleReOpened()
        {
            throw new System.NotImplementedException();
        }

        public void CycleReOpenedCancelation()
        {
            throw new System.NotImplementedException();
        }

        public Task ReOpen()
        {
            throw new System.NotImplementedException();
        }

        public Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, ITransportDataProvider dataProvider, CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }
    }
}