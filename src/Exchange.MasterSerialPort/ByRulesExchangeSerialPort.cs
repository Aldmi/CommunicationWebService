using System;
using System.Threading;
using System.Threading.Tasks;
using Exchange.MasterSerialPort.Option;
using Transport.SerialPort.Abstract;
using Worker.Background.Abstarct;

namespace Exchange.MasterSerialPort
{
    /// <summary>
    /// ПОВЕДЕНИЕ ОБМЕНА ДАННЫМИ ПО ПРАВИЛАМ ЗАДАНЫМ ИЗВНЕ ПО ПОСЛЕД. ПОРТУ
    /// </summary>
    public class ByRulesExchangeSerialPort : BaseExchangeSerialPort
    {
        #region prop

        public ByRulesExchangeSerialPort(ISerailPort serailPort, IBackgroundService backgroundService, ExchangeMasterSpOption exchangeMasterSpOption)
            : base(serailPort, backgroundService, exchangeMasterSpOption)
        {

        }

        #endregion




        #region OverrideMembers

        protected override async Task OneTimeExchangeActionAsync(CancellationToken ct)
        {
           await Task.Delay(1000, ct);
        }

        protected override async Task CycleTimeExchangeActionAsync(CancellationToken ct)
        {
            await Task.Delay(2000, ct);//DEBUG
            Console.WriteLine($"ExchangeMasterSpOption.PortName=  {ExchangeMasterSpOption.PortName}");//DEBUG
        }

        #endregion
    }
}