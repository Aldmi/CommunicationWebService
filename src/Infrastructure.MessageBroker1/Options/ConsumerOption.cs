using System.Collections.Generic;

namespace Infrastructure.MessageBroker.Options
{
    public class ConsumerOption
    {
        public string BrokerEndpoints { get; set; }
        public string GroupId { get; set; }
        public IEnumerable<string> Topics { get; set; }
    }
}