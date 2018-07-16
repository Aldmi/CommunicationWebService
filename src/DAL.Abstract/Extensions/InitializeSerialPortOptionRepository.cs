using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Transport;

namespace DAL.Abstract.Extensions
{
    public static class InitializeSerialPortOptionRepository
    {
        public static void Initialize(this ISerialPortOptionRepository rep)
        {
            var serials = new List<SerialOption>
            {
                new SerialOption
                {
                    Port = "COM1",
                    BaudRate = 9600,
                    DataBits = 8,
                    DtrEnable = false,
                    Parity = Parity.Even,
                    StopBits = StopBits.One,
                }
            };

            rep.AddRange(serials);
        }
    }
}