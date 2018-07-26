using System.Collections.Generic;

namespace DAL.Abstract.Entities.Transport
{

    public class TransportOption
    {
        public IEnumerable<SerialOption> SerialOptions { get; set; }
        public IEnumerable<TcpIpOption> TcpIpOptions { get; set; }
        public IEnumerable<HttpOption> HttpOptions { get; set; }
    }
}