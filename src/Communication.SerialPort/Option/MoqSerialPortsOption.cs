using System.Collections.Generic;

namespace Communication.SerialPort.Option
{
    /// <summary>
    /// Moq объект настроек для отладки.
    /// </summary>
    public static class MoqSerialPortsOption
    {
        public static SerialPortsOption GetSerialPortsOption()
        {
            return new SerialPortsOption
            {
                Serials = new List<SerialOption>
                {
                    new SerialOption
                    {
                        Port = "COM1",
                        BaudRate = 9600,
                        DataBits = 1,
                        DtrEnable = false,
                        Parity = Parity.Even,
                        StopBits = StopBits.One,
                    },
                    new SerialOption
                    {
                        Port = "COM2",
                        BaudRate = 9600,
                        DataBits = 1,
                        DtrEnable = false,
                        Parity = Parity.Even,
                        StopBits = StopBits.One,
                    }
                }
            };
        }
    }
}