using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;
using DAL.Abstract.Entities.Exchange;
using DAL.Abstract.Entities.Transport;
using Shared.Enums;
using Shared.Types;

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


        public bool AddDeviceOption(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions = null, TransportOption transportOption = null)
        {
            if (deviceOption == null && exchangeOptions == null && transportOption == null)
                return false;

            if (deviceOption != null && exchangeOptions == null && transportOption == null)
            {
                AddDeviceOptionWithExiststExchangeOptions(deviceOption);
            }
            else
            if (deviceOption != null && exchangeOptions != null && transportOption == null)
            {
                AddDeviceOptionWithNewExchangeOptions(deviceOption, exchangeOptions);
            }
            else 
            if (deviceOption != null && exchangeOptions != null)
            {
                AddDeviceOptionWithNewExchangeOptionsAndNewTransportOptions(deviceOption, exchangeOptions, transportOption);
            }

            return true;
        }


        /// <summary>
        /// Добавить девайс, который использует уже существующие обмены.
        /// Если хотябы 1 обмен из списка не найденн, то выкидываем Exception.
        /// </summary>
        private void AddDeviceOptionWithExiststExchangeOptions(DeviceOption deviceOption)
        {
            var exceptionStr = new StringBuilder();
            foreach (var exchangeKey in deviceOption.ExchangeKeys)
            {
                if (!_exchangeOptionRep.IsExist(exchangeOption => exchangeOption.Key == exchangeKey))
                {
                    exceptionStr.AppendFormat("{0}, ",exchangeKey);
                }
            }

            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new Exception($"Не найденны exchangeKeys:  {exceptionStr}");
            }

            _deviceOptionRep.Add(deviceOption);
        }


        /// <summary>
        /// Добавить девайс, для которого нужно создать 1 или несколько обменов.
        /// Если хотябы для 1 обмена из списка "exchangeOption", не найденн транспорт, то выкидываем Exception.
        /// Если обмен не существует, добавим его, если существует, то игнорируем добавление.
        /// </summary>
        private void AddDeviceOptionWithNewExchangeOptions(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions)
        {
            var exceptionStr = new StringBuilder();

            //ПРОВЕРКА СООТВЕТСТВИЯ exchangeKeys, УКАЗАННОЙ В deviceOption, КЛЮЧАМ ИЗ exchangeOptions
            var exchangeExternalKeys = exchangeOptions.Select(exchangeOption=> exchangeOption.Key).ToList();
            var diff= exchangeExternalKeys.Except(deviceOption.ExchangeKeys).ToList();
            if (diff.Count > 0)
            {
                throw new Exception("Найденно несоответсвие ключей указанных для Device, ключам в exchangeOptions");
            }

            //ПРОВЕРКА НАЛИЧИЯ УЖЕ СОЗДАНННОГО ТРАНСПОРТА ДЛЯ КАЖДОГО ОБМЕНА
            foreach (var exchangeOption in exchangeOptions)
            {
                if (!IsExistTransport(exchangeOption.KeyTransport))
                {
                    exceptionStr.AppendFormat("{0}, ", exchangeOption.KeyTransport);
                }
            }
            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new Exception($"Для ExchangeOption не найденн транспорт по ключу:  {exceptionStr}");
            }

            //ДОБАВИМ ТОЛЬКО НОВЫЕ ОБМЕНЫ К РЕПОЗИТОРИЮ ОБМЕНОВ
            foreach (var exchangeOption in exchangeOptions)
            {
                if (!_exchangeOptionRep.IsExist(exchOpt => exchOpt.Key == exchangeOption.Key))
                {
                    _exchangeOptionRep.Add(exchangeOption);
                }
            }

            //ДОБАВИТЬ ДЕВАЙС
            _deviceOptionRep.Add(deviceOption);
        }


        /// <summary>
        /// Добавить девайс, для которого нужно создать 1 или несколько обменов.
        /// Если обмен не существует, добавим его, если существует, то добавим существующий.
        /// Если для нового обменна не существует транспорт, то создадим транспорт.
        /// Если хотябы 1 транспорт в "transportOption" уже существует, то выкидываем Exception.
        /// </summary>
        private void AddDeviceOptionWithNewExchangeOptionsAndNewTransportOptions(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions, TransportOption transportOption)
        {

        }


        private bool IsExistTransport(KeyTransport keyTransport)
        {
            switch (keyTransport.TransportType)
            {
                case TransportType.SerialPort:
                    return _serialPortOptionRep.IsExist(sp=> sp.Port == keyTransport.Key);

                case TransportType.TcpIp:
                    return _tcpIpOptionRep.IsExist(tcpip=> tcpip.Name == keyTransport.Key);

                case TransportType.Http:
                    return _httpOptionRep.IsExist(http=> http.Name == keyTransport.Key);
            }

            return false;
        }

        #endregion
    }
}