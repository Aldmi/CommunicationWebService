using System;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Types;

namespace Transport.Http.Abstract
{
    public interface IHttp : ISupportKeyTransport, IDisposable
    {
        HttpOption Option { get; }                                                           //НАСТРОЙКИ Http
    }
}