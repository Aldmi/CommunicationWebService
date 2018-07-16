using System.Collections.Generic;

namespace DAL.Abstract.Entities.Device
{
    public class Device : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> ExchangesId { get; set; }
    }
}