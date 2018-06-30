using System.Collections.Generic;
using Communication.SerialPort.Option;

namespace Exchange.MasterSerialPort.Option
{
    public static class MoqExchangeMasterSerialPortOptions
    {
        public static ExchangeMasterSpOptions GetExchangeMasterSerialPortOptions() => new ExchangeMasterSpOptions
        {
            ExchangesMasterSp = new List<ExchangeMasterSpOption>
                {
                    new ExchangeMasterSpOption
                    {
                        PortName = "COM2",
                        Address = "1",
                        TimeResponse = 1000,
                        ExchangeRule = new ExchangeRule
                        {
                            Table = new Table
                            {
                                Position = 0,
                                Size = 8,
                                Rule = new Rule
                                {
                                    Format = "Windows-1251",
                                    Request = new Description
                                    {
                                        Body = "STX{AddressDevice:X2}{Nbyte:X2}{CRC16}",
                                        MaxLenght = 256
                                    },
                                    Response = new Description
                                    {
                                        Body = "STX{AddressDevice:X2}{Nbyte:X2}{CRC16}",
                                        MaxLenght = 8
                                    }
                                }
                            }
                        }
                    },
                    new ExchangeMasterSpOption
                    {
                        PortName = "COM1",
                        Address = "2",
                        TimeResponse = 1000,
                        ExchangeRule = new ExchangeRule
                        {
                            Table = new Table
                            {
                                Position = 0,
                                Size = 8,
                                Rule = new Rule
                                {
                                    Format = "Windows-1251",
                                    Request = new Description
                                    {
                                        Body = "STX{AddressDevice:X2}{Nbyte:X2}{CRC16}",
                                        MaxLenght = 256
                                    },
                                    Response = new Description
                                    {
                                        Body = "STX{AddressDevice:X2}{Nbyte:X2}{CRC16}",
                                        MaxLenght = 8
                                    }
                                }
                            }
                        }
                    }
                }
        };
    }
}