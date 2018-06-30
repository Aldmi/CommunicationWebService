using System.Threading;
using Exchange.Base;
using Shared.Enums;
using Transport.SerialPort.Abstract;
using Worker.Background.Abstarct;


namespace Exchange.MasterSerialPort
{
    public class BaseExchangeSerialPort : IExhangeBehavior
    {
        #region field

        private readonly ISerailPort _serailPort;
        private readonly IBackgroundService _backgroundService;

        #endregion




        #region prop

        public TypeExchange TypeExchange => TypeExchange.SerialPort;

        #endregion





        #region ctor

        public BaseExchangeSerialPort(ISerailPort serailPort, IBackgroundService backgroundService)
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