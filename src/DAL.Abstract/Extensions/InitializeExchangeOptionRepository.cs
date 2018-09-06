using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Exchange.Providers;
using Shared.Enums;
using Shared.Types;

namespace DAL.Abstract.Extensions
{
    public static class InitializeExchangeOptionRepository
    {
        public static async Task InitializeAsync(this IExchangeOptionRepository rep)
        {
            //Если есть хотя бы 1 элемент то НЕ иннициализировать
            if (await rep.CountAsync(option=> true) > 0) 
            {
                return;
            }

            var exchanges = new List<ExchangeOption>
            {
                new ExchangeOption
                {
                    Id = 1,
                    Key = "SP_COM1_Vidor1",
                    KeyTransport = new KeyTransport("COM1", TransportType.SerialPort),
                    Provider = new ProviderOption
                    {
                        Name = "VidorBinary",
                        ManualProviderOption = new ManualProviderOption
                        {
                            Address = "100",
                            TimeRespone = 2500
                        }
                    }
                },
                new ExchangeOption
                {
                    Id = 2,
                    Key = "SP_COM2_Vidor2",
                    KeyTransport = new KeyTransport("COM2", TransportType.SerialPort),
                    Provider = new ProviderOption
                    {
                        Name = "VidorBinary",
                        ManualProviderOption = new ManualProviderOption
                        {
                            Address = "100",
                            TimeRespone = 2500
                        }
                    }
                },
                new ExchangeOption
                {
                    Id = 3,
                    Key = "HTTP_google.com_Table1",
                    KeyTransport = new KeyTransport("http:\\google.com", TransportType.Http),
                    Provider = new ProviderOption
                    {
                        Name = "VidorBinary",
                        ManualProviderOption = new ManualProviderOption
                        {
                            Address = "100",
                            TimeRespone = 2500
                        }
                    }
                }
            };

           await rep.AddRangeAsync(exchanges);
        }
    }
}
