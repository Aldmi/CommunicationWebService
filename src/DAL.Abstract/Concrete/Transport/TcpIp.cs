using DAL.Abstract.Entities;

namespace DAL.Abstract.Concrete.Transport
{
    public class TcpIp : EntityBase
    {
        public string Name { get; set; }
    }
}