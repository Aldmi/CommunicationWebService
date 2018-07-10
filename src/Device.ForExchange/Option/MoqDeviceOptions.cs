using System.Collections.Generic;

namespace Device.ForExchange.Option
{
    public static class MoqDeviceOptions
    {
        public static DeviceOptions GetExchangeMasterSerialPortOptions() => new DeviceOptions
        {
            Options= new List<DeviceOption>
            {
                new DeviceOption()
            }
        };



    }
}