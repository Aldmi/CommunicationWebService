using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Enums;
using Shared.Types;
using Transport.Base.DataProvidert;
using Transport.Base.RxModel;
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

        public bool IsOpen { get; }
        public string StatusString { get; }
        public StatusDataExchange StatusDataExchange { get; }
        public bool IsCycleReopened { get; }
        public ISubject<IsOpenChangeRxModel> IsOpenChangeRx { get; }
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeRx { get; }
        public ISubject<StatusStringChangeRxModel> StatusStringChangeRx { get; }
        public async Task<bool> CycleReOpened()
        {
            await Task.CompletedTask;
            return true;
        }

        public void CycleReOpenedCancelation()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ReOpen()
        {
            throw new System.NotImplementedException();
        }

        public Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, ITransportDataProvider dataProvider, CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }
    }
}