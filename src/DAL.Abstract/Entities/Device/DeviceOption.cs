using System.Collections.Generic;

namespace DAL.Abstract.Entities.Device
{
    public class DeviceOption : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> ExchangesId { get; set; }   //TODO: заменить на коллекцию ключей (KeyTransport) для ссылки на ExchangeOption.
    }
}