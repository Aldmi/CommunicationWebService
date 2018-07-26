using System.Collections.Generic;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;
using DAL.Abstract.Entities.Exchange;
using DAL.Abstract.Entities.Transport;

namespace BL.Services.Mediators
{
    /// <summary>
    /// Сервис объединяет работу с репозиотриями опций для устройств.
    /// DeviceOption + ExchangeOption + TransportOption.
    /// 
    /// </summary>
    public class MediatorForOptionsRepository
    {
        #region fields

        private readonly IDeviceOptionRepository _deviceOptionRep;
        private readonly IExchangeOptionRepository _exchangeOptionRep;
        private readonly ISerialPortOptionRepository _serialPortOptionRep;
        private readonly ITcpIpOptionRepository _tcpIpOptionRep;
        private readonly IHttpOptionRepository _httpOptionRep;

        #endregion





        #region ctor

        public MediatorForOptionsRepository(IDeviceOptionRepository deviceOptionRep,
            IExchangeOptionRepository exchangeOptionRep,
            ISerialPortOptionRepository serialPortOptionRep,
            ITcpIpOptionRepository tcpIpOptionRep,
            IHttpOptionRepository httpOptionRep)
        {
            _deviceOptionRep = deviceOptionRep;
            _exchangeOptionRep = exchangeOptionRep;
            _serialPortOptionRep = serialPortOptionRep;
            _tcpIpOptionRep = tcpIpOptionRep;
            _httpOptionRep = httpOptionRep;
        }

        #endregion





        #region Methode

        public IEnumerable<DeviceOption> GetDeviceOptions()
        {
            return _deviceOptionRep.List();
        }


        public IEnumerable<ExchangeOption> GetExchangeOptions()
        {
            return _exchangeOptionRep.List();
        }


        public TransportOption GetTransportOptions()
        {
            return new TransportOption
            {
                SerialOptions = _serialPortOptionRep.List(),
                TcpIpOptions = _tcpIpOptionRep.List(),
                HttpOptions = _httpOptionRep.List()
            };
        }

        #endregion




    }
}