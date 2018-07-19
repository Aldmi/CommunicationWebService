using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;

namespace DAL.Abstract.Extensions
{
    public static class InitializeDeviceOptionRepository
    {
        public static void Initialize(this IDeviceOptionRepository rep)
        {
            var devices = new List<DeviceOption>
            {
                new DeviceOption
                {
                    Id = 1,
                    Description = "Табло1",
                    Name = "Vidor1",
                    ExchangesId = new List<int>
                    {
                        1
                    }
                }
            };
            rep.AddRange(devices);
        }
    }
}