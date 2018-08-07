using DAL.Abstract.Entities.Options.Transport;
using Shared.Types;
using Transport.Http.Abstract;

namespace Transport.Http.Concrete
{
    public class HttpTransport : IHttp
    {
        #region prop

        public KeyTransport KeyTransport { get; }
        public HttpOption Option { get; }

        #endregion




        #region ctor

        public HttpTransport(HttpOption option, KeyTransport keyTransport)
        {
            KeyTransport = keyTransport;
            Option = option;
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
           
        }

        #endregion
    }
}