using System;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Types;

namespace Transport.TcpIp.Abstract
{
    public interface ITcpIp : ISupportKeyTransport, IDisposable
    {
        TcpIpOption Option { get; }                                                           //НАСТРОЙКИ TcpIp
    }
}