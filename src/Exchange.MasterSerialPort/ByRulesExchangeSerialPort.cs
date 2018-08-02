using System;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange;
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

        public ByRulesExchangeSerialPort(ISerailPort serailPort, IBackground background, ExchangeOption exchangeOption)
            : base(serailPort, background, exchangeOption)
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
            Console.WriteLine($"ExchangeOption.KeyTransport.Key=  {ExchangeOption.KeyTransport.Key}");//DEBUG
        }

        #endregion
    }
}