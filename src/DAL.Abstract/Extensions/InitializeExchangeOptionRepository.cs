using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Exchange;
using Shared.Enums;
using Shared.Types;

namespace DAL.Abstract.Extensions
{
    public static class InitializeExchangeOptionRepository
    {
        public static void Initialize(this IExchangeOptionRepository rep)
        {
            var exchanges = new List<ExchangeOption>
            {
                new ExchangeOption
                {
                    Key = "SP_COM1_Vidor1",
                    Id = 1,
                    KeyTransport = new KeyTransport("COM1", TransportType.SerialPort),
                    Provider = new Provider
                    {
                        Id = 1,
                        Address = "10",
                        Name = "VidorBase",
                        TimeRespone = 500
                    }
                },
                new ExchangeOption
                {
                    Key = "SP_COM2_Vidor2",
                    Id = 2,
                    KeyTransport = new KeyTransport("COM2", TransportType.SerialPort),
                    Provider = new Provider
                    {
                        Id = 1,
                        Address = "12",
                        Name = "Table",
                        TimeRespone = 500
                    }
                },
                new ExchangeOption
                {
                    Key = "HTTP_google.com_Table1",
                    Id = 2,
                    KeyTransport = new KeyTransport("http:\\google.com", TransportType.Http),
                    Provider = new Provider
                    {
                        Id = 1,
                        Address = "http:\\google.com",
                        Name = "VidorBase",
                        TimeRespone = 500
                    }
                }
            };

            rep.AddRange(exchanges);
        }
    }
}
