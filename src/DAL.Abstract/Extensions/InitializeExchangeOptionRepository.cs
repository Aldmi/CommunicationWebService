using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Exchange;
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
                    Id = 1,
                    KeyTransport = new KeyTransport("COM1", TransportType.SerialPort),
                    Provider = new Provider
                    {
                        Id = 1,
                        Address = "10",
                        Name = "VidorBase",
                        TimeRespone = 500
                    }
                }
            };

            rep.AddRange(exchanges);
        }
    }
}
