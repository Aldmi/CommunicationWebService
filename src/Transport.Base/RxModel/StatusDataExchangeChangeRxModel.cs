using Shared.Enums;

namespace Transport.Base.RxModel
{
    public class StatusDataExchangeChangeRxModel
    {
        public StatusDataExchange StatusDataExchange { get; set; }
        public string TransportName { get; set; }
        public string KeyExchange { get; set; }
    }
}