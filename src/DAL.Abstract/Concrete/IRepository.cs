﻿using DAL.Abstract.Abstract;
using DAL.Abstract.Entities.Transport;

namespace DAL.Abstract.Concrete
{
    /// <summary>
    /// Доступ к транспорту послед. порты
    /// </summary>
    public interface ISerialPortRepository : IGenericDataRepository<Serial>
    {  
    }

    /// <summary>
    /// Доступ к транспорту TcpIp
    /// </summary>
    public interface ITcpIpRepository : IGenericDataRepository<TcpIp>
    {
    }

    /// <summary>
    /// Доступ к транспорту Http
    /// </summary>
    public interface IHttpRepository : IGenericDataRepository<Http>
    {
    }


    /// <summary>
    /// Доступ Exchange
    /// </summary>
    public interface IExchangeRepository : IGenericDataRepository<Entities.Exchange.Exchange>
    {
    }

}