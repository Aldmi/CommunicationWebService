using System.Collections.Generic;
using Shared.Types;

namespace DAL.Abstract.Entities.Device
{
    public class DeviceOption : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool AutoBuild { get; set; }                         //Автоматичекое создание Deivice на базе DeviceOption, при запуске сервиса.
        public bool AutoStart{ get; set; }                          //Автоматичекий запук Deivice в работу (после AutoBuild), при запуске сервиса.
        public List<string> ExchangeKeys { get; set; }
    }
}