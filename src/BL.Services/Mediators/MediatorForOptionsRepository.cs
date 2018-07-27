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


        public bool AddDeviceOption(DeviceOption deviceOption, ExchangeOption exchangeOption, TransportOption transportOption)
        {
            if (deviceOption == null && exchangeOption == null && transportOption == null)
                return false;

            if (deviceOption != null && exchangeOption == null && transportOption == null)
            {
                AddDeviceOptionWithExiststExchangeOptions(deviceOption);
            }
            else
            if (deviceOption != null && exchangeOption != null && transportOption == null)
            {
                AddDeviceOptionWithNewExchangeOptions(deviceOption, exchangeOption);
            }
            else 
            if (deviceOption != null && exchangeOption != null)
            {
                AddDeviceOptionWithNewExchangeOptionsAndNewTransportOptions(deviceOption, exchangeOption, transportOption);
            }

            return true;
        }


        /// <summary>
        /// Добавить девайс, который использует уже существующие обмены.
        /// Если хотябы 1 обмен из списка не найденн, то выкидываем Exception
        /// </summary>
        private void AddDeviceOptionWithExiststExchangeOptions(DeviceOption deviceOption)
        {

        }


        /// <summary>
        /// Добавить девайс, для которого нужно создать 1 или несколько обменов.
        /// Если обмен не существует, добавим его, если существует, то добавим существующий.
        /// Если хотябы для 1 обмена из списка "exchangeOption", не найденн транспорт, то выкидываем Exception.
        /// </summary>
        private void AddDeviceOptionWithNewExchangeOptions(DeviceOption deviceOption, ExchangeOption exchangeOption)
        {

        }


        /// <summary>
        /// Добавить девайс, для которого нужно создать 1 или несколько обменов.
        /// Если обмен не существует, добавим его, если существует, то добавим существующий.
        /// Если для нового обменна не существует транспорт, то создадим транспорт.
        /// Если хотябы 1 транспорт в "transportOption" уже существует, то выкидываем Exception.
        /// </summary>
        private void AddDeviceOptionWithNewExchangeOptionsAndNewTransportOptions(DeviceOption deviceOption, ExchangeOption exchangeOption, TransportOption transportOption)
        {

        }

        #endregion
    }
}