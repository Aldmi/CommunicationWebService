using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Device;
using Shared.Enums;
using Shared.Types;

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
                    ExchangeKeys = new List<string>
                    {
                        "SP_COM1_Vidor1",
                        "SP_COM2_Vidor2"
                    }
                }
            };
            rep.AddRange(devices);
        }
    }
}