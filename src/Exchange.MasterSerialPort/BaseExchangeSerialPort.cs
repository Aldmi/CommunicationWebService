using System.Threading;
using Communication.SerialPort.Abstract;
using Exchange.Base;
using Worker.Background.Abstarct;


namespace Exchange.MasterSerialPort
{
    public class BaseExchangeSerialPort : IExhangeBehavior
    {
        private readonly ISerailPort _serailPort;
        private readonly IBackgroundService _backgroundService;




        #region ctor

        public BaseExchangeSerialPort(ISerailPort serailPort, IBackgroundService backgroundService )
        {
            _serailPort = serailPort;
            _backgroundService = backgroundService;
        }

        #endregion




        public void StartCycleExchange()
        {
            _backgroundService.StartAsync(CancellationToken.None);
        }

        public void StopCycleExchange()
        {
            _backgroundService.StopAsync(CancellationToken.None);
        }
    }
}