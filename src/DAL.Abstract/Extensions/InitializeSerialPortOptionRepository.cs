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
            //Если есть хотя бы 1 элемент то НЕ иннициализировать
            if (await rep.CountAsync(option=> true) > 0) 
            {
                return;
            }

            //var serials = new List<SerialOption>
            //{
            //    new SerialOption
            //    {
            //        Id = 1,
            //        Port = "COM1",
            //        BaudRate = 9600,
            //        DataBits = 8,
            //        DtrEnable = false,
            //        Parity = Parity.Even,
            //        StopBits = StopBits.One,
            //        AutoStart = true
            //    },
            //    //new SerialOption
            //    //{
            //    //    Id = 2,
            //    //    Port = "COM2",
            //    //    BaudRate = 9600,
            //    //    DataBits = 8,
            //    //    DtrEnable = false,
            //    //    Parity = Parity.Even,
            //    //    StopBits = StopBits.One,
            //    //    AutoStart = true
            //    //}
            //};

           //await rep.AddRangeAsync(serials);
        }
    }
}