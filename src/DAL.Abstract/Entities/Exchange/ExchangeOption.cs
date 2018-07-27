using System.Collections.Generic;
using Shared.Types;

namespace DAL.Abstract.Entities.Exchange
{
    public class ExchangeOption : EntityBase
    {
        public string Key { get; set; }
        public KeyTransport KeyTransport { get; set; }
        public ExchangeRule ExchangeRule { get; set; }
        public Provider Provider { get; set; }
    }
}