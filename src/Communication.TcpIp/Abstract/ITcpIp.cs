using System;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Types;
using Transport.Base.Abstract;

namespace Transport.TcpIp.Abstract
{
    public interface ITcpIp : ITransport
    {
        TcpIpOption Option { get; }                                                           //НАСТРОЙКИ TcpIp
    }
}