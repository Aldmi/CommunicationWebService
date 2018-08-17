using System.Collections.Generic;

namespace DAL.Abstract.Entities.Options.Transport
{
    public class HttpOption : BackgroundOption
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}