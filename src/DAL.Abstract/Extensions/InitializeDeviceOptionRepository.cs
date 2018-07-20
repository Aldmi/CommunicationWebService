using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;
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
                    ExchangeKeys = new List<KeyTransport>
                    {
                        new KeyTransport("COM1", TransportType.SerialPort),
                        new KeyTransport("COM2", TransportType.SerialPort)
                    }
                }
            };
            rep.AddRange(devices);
        }
    }
}