using System.Collections.Generic;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.Abstract.Entities.Options
{
    public class OptionAgregator
    {
        public List<DeviceOption> DeviceOptions { get; set; }
        public List<ExchangeOption> ExchangeOptions { get; set; }
        public TransportOption TransportOptions { get; set; }
    }
}