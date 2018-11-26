using System.Collections.Generic;
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
                //new ExchangeOption
                //{
                //    Id = 1,
                //    Key = "SP_COM1_Vidor1",
                //    KeyTransport = new KeyTransport("COM1", TransportType.SerialPort),
                //    AutoStartCycleFunc = false,
                //    CountBadTrying = 3,
                //    Provider = new ProviderOption
                //    {
                //        Name = "ByRules",
                //        ByRulesProviderOption = new ByRulesProviderOption
                //        {
                //            Rules = new List<RuleOption>
                //            {
                //                new RuleOption
                //                {
                //                    Name = "Rule_1",
                //                    AddressDevice = "10",
                //                    //WhereFilter = "(TypeTrain == \"Suburban\") && (PathNumber == \"2\" || PathNumber == \"3\" || PathNumber == \"4\")",
                //                    WhereFilter = "true",
                //                    OrderBy = "Id",
                //                    //OrderBy = "ArrivalTime",
                //                    TakeItems = 1, //2
                                    
                //                    ViewRules = new List<ViewRuleOption>
                //                    {
                //                        new ViewRuleOption
                //                        {
                //                            Id = 1,
                //                            StartPosition = 0,
                //                            Count = 1,
                //                            BatchSize = 1000,
                //                            RequestOption = new RequestOption{Header = "{adress}", Body = "01{adress}0502{Station}", Footer = "{CrcXor16}", MaxBodyLenght = 140, Format = "Windows-1251"},
                //                            ResponseOption = new ResponseOption{Body = "01050A", Lenght = 3, TimeRespone = 3000, Format = "X"}
                //                        },
                //                        //new ViewRuleOption
                //                        //{
                //                        //    Id = 2,
                //                        //    StartPosition = 1,
                //                        //    Count = 1,
                //                        //    BatchSize = 1000,
                //                        //    RequestOption = new RequestOption{Header = "{adress}", Body = "01{adress}05026{ArrivalTime}2158{Station}", Footer = "{CrcXor16}", MaxLenght = 1000, Format = "Windows-1251"},
                //                        //    ResponseOption = new ResponseOption{Body = "01050A0606", MaxLenght = 2000, TimeRespone = 3000, Format = "X"}
                //                        //}
                //                    }
                //                }
                //            }
                //        }
                //    }
                //},
                new ExchangeOption
                {
                    Id = 2,
                    Key = "TcpIp_table_1",
                    KeyTransport = new KeyTransport("TcpIp table 1", TransportType.TcpIp),
                    AutoStartCycleFunc = false, // DEBUG
                    CountBadTrying = 10,
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
                                    AddressDevice = "46", //59
                                    //WhereFilter = "(TypeTrain == \"Suburban\") && (PathNumber == \"2\" || PathNumber == \"3\" || PathNumber == \"4\")",
                                    WhereFilter = "true",
                                    OrderBy = "Id",
                                    //OrderBy = "ArrivalTime",
                                    TakeItems = 1, //2
                                    DefaultItemJson= "{\"pathNumber\": \"5\"}",  //"{}" - дефолтный конструктор типа
                                    ViewRules = new List<ViewRuleOption>
                                    {
                                        new ViewRuleOption
                                        {
                                            Id = 1,
                                            StartPosition = 0,
                                            Count = 1,
                                            BatchSize = 1000,
                                            RequestOption = new RequestOption
                                            {
                                                Header = "\u0002{AddressDevice:X2}{Nbyte:X2}",
                                                Body = "%0100002300000f0000001E%10{NumberOfCharacters:X2}05\\\"{NumberOfTrain}\\\"",
                                                Footer = "{CRCXor:X2}\u0003",
                                                MaxBodyLenght = 200,
                                                Format = "Windows-1251"
                                            },
                                            ResponseOption = new ResponseOption
                                            {
                                                Body = "0246463038254130373741434B454103",
                                                Lenght = 16,
                                                TimeRespone = 3000,
                                                Format = "X2"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

           await rep.AddRangeAsync(exchanges);
        }
    }
}

