using System.Collections.Generic;

namespace DAL.Abstract.Entities.Transport
{
    public class Http : EntityBase
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}