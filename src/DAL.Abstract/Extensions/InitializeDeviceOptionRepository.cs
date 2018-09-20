using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Device;

namespace DAL.Abstract.Extensions
{
    public static class InitializeDeviceOptionRepository
    {
        public static async Task InitializeAsync(this IDeviceOptionRepository rep)
        {
            //Если есть хотя бы 1 элемент то НЕ иннициализировать
            if (await rep.CountAsync(option=> true) > 0) 
            {
                return;
            }

            var devices = new List<DeviceOption>
            {
                new DeviceOption
                {
                    Id = 1,
                    Description = "Табло1",
                    Name = "Vidor1",
                    AutoBuild = false,
                    ExchangeKeys = new List<string>
                    {
                        "SP_COM1_Vidor1",
                        "SP_COM2_Vidor2"
                    }
                },
                new DeviceOption
                {
                    Id = 2,
                    Description = "Табло2",
                    Name = "HttpTable_google",
                    AutoBuild = false,
                    ExchangeKeys = new List<string>
                    {
                        "HTTP_google.com_Table1"
                    }
                },
                new DeviceOption
                {
                    Id = 3,
                    Description = "Табло3",
                    Name = "TcpIp Table1",
                    AutoBuild = false,
                    ExchangeKeys = new List<string>
                    {
                        "TcpIp_Table1"
                    }
                }
            };
            await rep.AddRangeAsync(devices);
        }
    }
}