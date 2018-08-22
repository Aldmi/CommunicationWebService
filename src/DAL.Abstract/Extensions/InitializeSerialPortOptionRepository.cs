using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.Abstract.Extensions
{
    public static class InitializeSerialPortOptionRepository
    {
        public static async Task InitializeAsync(this ISerialPortOptionRepository rep)
        {
            //Если есть хотя бы 1 элемент то выйти

            if (rep.GetSingle(option => true) != null) //TODO: 
            {
                return;
            }

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
                },
                new SerialOption
                {
                    Port = "COM2",
                    BaudRate = 9600,
                    DataBits = 8,
                    DtrEnable = false,
                    Parity = Parity.Even,
                    StopBits = StopBits.One,
                }
            };

           await rep.AddRangeAsync(serials);
        }
    }
}