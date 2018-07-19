using System.Collections.Generic;

namespace DAL.Abstract.Entities.Exchange
{
    public class ExchangeOption : EntityBase
    {
        public Dictionary<string, string> KeyTransport { get; set; }
        public ExchangeRule ExchangeRule { get; set; }
        public Provider Provider { get; set; }
    }
}