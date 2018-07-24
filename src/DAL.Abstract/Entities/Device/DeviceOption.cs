using System.Collections.Generic;
using Shared.Types;

namespace DAL.Abstract.Entities.Device
{
    public class DeviceOption : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<KeyTransport> KeyTransports { get; set; }
    }
}