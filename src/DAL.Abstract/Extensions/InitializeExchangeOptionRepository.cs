﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
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
                    AutoStartCycleFunc = false,
                    CountBadTrying = 3,
                    Provider = new ProviderOption
                    {
                        Name = "ByRules",
                        ByRulesProviderOption = new ByRulesProviderOption
                        {
                            Rules = new List<RuleOption>
                            {
                                new RuleOption
                                {
                                    Name = "Rule_1",
                                    BatchSize = 1,
                                    RequestOption = new RequestOption{Body = "01{adress}0502{Station}", MaxLenght = 1000, Format = "Windows-1251"},
                                    ResponseOption = new ResponseOption{Body = "01050A", MaxLenght = 2000, TimeRespone = 3000, Format = "X"}
                                }
                            }
                        }
                    }
                },
                //new ExchangeOption
                //{
                //    Id = 2,
                //    Key = "SP_COM2_Vidor2222",
                //    KeyTransport = new KeyTransport("COM2", TransportType.SerialPort),
                //    AutoStartCycleFunc = false,
                //    Provider = new ProviderOption
                //    {
                //        Name = "VidorBinary",
                //        ManualProviderOption = new ManualProviderOption
                //        {
                //            Address = "100",
                //            TimeRespone = 2500
                //        }
                //    }
                //}
            };

           await rep.AddRangeAsync(exchanges);
        }
    }
}

