using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Exchange;

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
                    KeyTransport = new Dictionary<string, string>
                    {
                        {"Type", "Serial"},
                        {"Key", "COM1"}
                    },
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