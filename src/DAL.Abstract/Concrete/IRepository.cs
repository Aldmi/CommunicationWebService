using DAL.Abstract.Abstract;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.Abstract.Concrete
{
    /// <summary>
    /// Доступ к транспорту послед. порты
    /// </summary>
    public interface ISerialPortOptionRepository : IGenericDataRepository<SerialOption>
    {  
    }

    /// <summary>
    /// Доступ к транспорту TcpIp
    /// </summary>
    public interface ITcpIpOptionRepository : IGenericDataRepository<TcpIpOption>
    {
    }

    /// <summary>
    /// Доступ к транспорту Http
    /// </summary>
    public interface IHttpOptionRepository : IGenericDataRepository<HttpOption>
    {
    }


    /// <summary>
    /// Доступ Exchange
    /// </summary>
    public interface IExchangeOptionRepository : IGenericDataRepository<ExchangeOption>
    {
    }


    
    /// <summary>
    /// Доступ Device
    /// </summary>
    public interface IDeviceOptionRepository : IGenericDataRepository<DeviceOption>
    {
    }

}