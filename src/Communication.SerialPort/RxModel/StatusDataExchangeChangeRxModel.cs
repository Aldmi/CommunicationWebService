using Shared.Enums;

namespace Transport.SerialPort.RxModel
{
    public class StatusDataExchangeChangeRxModel
    {
        public StatusDataExchange StatusDataExchange { get; set; }
        public string PortName { get; set; }
    }
}