using System.Collections.Generic;
using Shared.Types;

namespace Device.ForExchange.Option
{
    public class DeviceOptions
    {
        public List<DeviceOption> Options { get; set; }
    }


    public class DeviceOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<KeyExchange> KeyExchanges { get; set; }
    }


}