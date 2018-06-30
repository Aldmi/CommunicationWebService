using System.Collections.Generic;

namespace CoreWithAutoFack.SettingsCommunication.Model
{

    public class HttpDevices
    {
        public List<HttpDevice> CollectionHttpDevice { get; set; }
    }


    public class HttpDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}