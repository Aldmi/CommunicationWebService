using System.Collections.Generic;

namespace DAL.Abstract.Entities.Options.Transport
{

    public class TransportOption
    {
        public IList<SerialOption> SerialOptions { get; set; }
        public IList<TcpIpOption> TcpIpOptions { get; set; }
        public IList<HttpOption> HttpOptions { get; set; }
    }
}