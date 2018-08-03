using DAL.Abstract.Entities.Options.Transport;
using Shared.Types;
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
    }
}