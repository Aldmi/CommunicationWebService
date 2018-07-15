using System.Collections.Generic;
using DAL.Abstract.Entities;

namespace DAL.Abstract.Concrete.Transport
{
    public class Http : EntityBase
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}