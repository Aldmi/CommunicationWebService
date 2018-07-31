using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BL.Services.Mediators.Exceptions;
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

        /// <summary>
        /// Вернуть все опции устройств из репозитория.
        /// </summary>
        public IEnumerable<DeviceOption> GetDeviceOptions()
        {
            return _deviceOptionRep.List();
        }


        /// <summary>
        /// Вернуть опции одного устройства по имени, из репозитория.
        /// </summary>
        public DeviceOption GetDeviceOptionByName(string deviceName)
        {
            var deviceOption = _deviceOptionRep.GetSingle(option => option.Name == deviceName);
            return deviceOption;
        }


        /// <summary>
        /// Вернуть все опции обменов из репозитория.
        /// </summary>
        public IEnumerable<ExchangeOption> GetExchangeOptions()
        {
            return _exchangeOptionRep.List();
        }


        /// <summary>
        /// Вернуть опции одного обмена из репозитория.
        /// </summary>
        public ExchangeOption GetExchangeByKey(string exchangeKey)
        {
            return _exchangeOptionRep.GetSingle(option => option.Key == exchangeKey);
        }


        /// <summary>
        /// Вернуть опции всех транспоротов из репозиториев.
        /// </summary>
        public TransportOption GetTransportOptions()
        {
            return new TransportOption
            {
                SerialOptions = _serialPortOptionRep.List(),
                TcpIpOptions = _tcpIpOptionRep.List(),
                HttpOptions = _httpOptionRep.List()
            };
        }


        /// <summary>
        /// Вернуть опции для спсика транспортов, по списку ключей из репозитория.
        /// </summary>
        public TransportOption GetTransportByKeys(IEnumerable<KeyTransport> keyTransports)
        {
            var serialOptions = new List<SerialOption>();
            var tcpIpOptions = new List<TcpIpOption>();
            var httpOptions = new List<HttpOption>();

            foreach (var keyTransport in keyTransports)
            {
                switch (keyTransport.TransportType)
                {
                    case TransportType.SerialPort:
                        serialOptions.Add(_serialPortOptionRep.GetSingle(option => option.Port == keyTransport.Key));
                        break;
                    case TransportType.TcpIp:
                        tcpIpOptions.Add(_tcpIpOptionRep.GetSingle(option => option.Name == keyTransport.Key));
                        break;
                    case TransportType.Http:
                        httpOptions.Add(_httpOptionRep.GetSingle(option => option.Name == keyTransport.Key));
                        break;
                }
            }
            return new TransportOption
            {
                SerialOptions = serialOptions,
                TcpIpOptions = tcpIpOptions,
                HttpOptions = httpOptions
            };
        }


        /// <summary>
        /// Добавить опции для устройства в репозиторий.
        /// Если exchangeOptions и transportOption не указанны, то добавляетя устройство с существующими обменами
        /// Если transportOption не указанн, то добавляетя устройство вместе со списом обменов, на уже существйющем транспорте.
        /// Если указзанны все аргументы, то добавляется устройство со спсиком новых обменом и каждый обмен использует новый транспорт.
        /// </summary>
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
        /// Удалим устройство.
        /// Если ключ обмена уникален, то удалим обмен, если в удаляемом обмене уникальный транспорт, то удалим транспорт.
        /// </summary>
        public bool RemoveDeviceOption(DeviceOption deviceOption)
        {  
            //ПРОВЕРКА УНИКАЛЬНОСТИ ОБМЕНОВ УДАЛЯЕМОГО УСТРОЙСТВА (ЕСЛИ УНИКАЛЬНО ТО УДАЛЯЕМ И ОБМЕН, ЕСЛИ ОБМЕН ИСПОЛЬУЕТ УНИКАЛЬНЫЙ ТРАНСПОРТ, ТО УДАЛЯЕМ И ТРАНСПОРТ)
            var exchangeKeys= _deviceOptionRep.List().SelectMany(option=> option.ExchangeKeys).ToList(); //уникальные ключи обменов со всех устройств.
            foreach (var exchangeKey in deviceOption.ExchangeKeys)
            {
                if (exchangeKeys.Count(key=> key == exchangeKey) == 1)                                                              //найден обмен используемый только этим устройством
                {
                   var singleExchOption= _exchangeOptionRep.GetSingle(exc=> exc.Key == exchangeKey);             
                   if (_exchangeOptionRep.List().Count(option => option.KeyTransport.Equals(singleExchOption.KeyTransport)) == 1)  //найденн транспорт используемый только этим (удаленным) обменом
                   {
                        RemoveTransport(singleExchOption.KeyTransport);                                                            //Удалить транспорт
                   }
                    _exchangeOptionRep.Delete(singleExchOption);                                                                    //Удалить обмен
                }
            }

            //УДАЛИМ УСТРОЙСТВО
            _deviceOptionRep.Delete(deviceOption);
            return true;
        }



        /// <summary>
        /// Добавить девайс, который использует уже существующие обмены.
        /// Если хотябы 1 обмен из списка не найденн, то выкидываем Exception.
        /// </summary>
        private void AddDeviceOptionWithExiststExchangeOptions(DeviceOption deviceOption)
        {
            //ПРОВЕРКА ОТСУТСВИЯ УСТРОЙСТВА по имени
            if (IsExistDevice(deviceOption.Name))
            {
                throw new OptionHandlerException($"Устройство с таким именем уже существует:  {deviceOption.Name}");
            }

            var exceptionStr = new StringBuilder();
            foreach (var exchangeKey in deviceOption.ExchangeKeys)
            {
                if (!_exchangeOptionRep.IsExist(exchangeOption => exchangeOption.Key == exchangeKey))
                {
                    exceptionStr.AppendFormat("{0}, ", exchangeKey);
                }
            }

            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new OptionHandlerException($"Не найденны exchangeKeys:  {exceptionStr}");
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
            //ПРОВЕРКА ОТСУТСВИЯ УСТРОЙСТВА по имени
            if (IsExistDevice(deviceOption.Name))
            {
                throw new OptionHandlerException($"Устройство с таким именем уже существует:  {deviceOption.Name}");
            }

            var exceptionStr = new StringBuilder();
            //ПРОВЕРКА СООТВЕТСТВИЯ exchangeKeys, УКАЗАННОЙ В deviceOption, КЛЮЧАМ ИЗ exchangeOptions
            var exchangeExternalKeys = exchangeOptions.Select(exchangeOption => exchangeOption.Key).ToList();
            var diff = exchangeExternalKeys.Except(deviceOption.ExchangeKeys).ToList();
            if (diff.Count > 0)
            {
                throw new OptionHandlerException("Найденно несоответсвие ключей указанных для Device, ключам указанным в exchangeOptions");
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
                throw new OptionHandlerException($"Для ExchangeOption не найденн транспорт по ключу:  {exceptionStr}");
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
            //ПРОВЕРКА ОТСУТСВИЯ УСТРОЙСТВА по имени
            if (IsExistDevice(deviceOption.Name))
            {
                throw new OptionHandlerException($"Устройство с таким именем уже существует:  {deviceOption.Name}");
            }

            var exceptionStr = new StringBuilder();
            //ПРОВЕРКА СООТВЕТСТВИЯ exchangeKeys, УКАЗАННОЙ В deviceOption, КЛЮЧАМ ИЗ exchangeOptions
            var exchangeExternalKeys = exchangeOptions.Select(exchangeOption => exchangeOption.Key).ToList();
            var diff = exchangeExternalKeys.Except(deviceOption.ExchangeKeys).ToList();
            if (diff.Count > 0)
            {
                throw new OptionHandlerException("Найденно несоответсвие ключей указанных для Device, ключам указанным в exchangeOptions");
            }

            //ПРОВЕРКА СООТВЕТСТВИЯ ключей exchangeOptions, указанному транспорту transportOption
            var keysByExchange = exchangeOptions.Select(option => option.KeyTransport);
            var keysBySp = transportOption.SerialOptions.Select(option => new KeyTransport(option.Port, TransportType.SerialPort));
            var keysByTcpIp = transportOption.TcpIpOptions.Select(option => new KeyTransport(option.Name, TransportType.TcpIp));
            var keysByHttp = transportOption.HttpOptions.Select(option => new KeyTransport(option.Name, TransportType.Http));
            var keysAllTransport = new List<KeyTransport>();
            keysAllTransport.AddRange(keysBySp);
            keysAllTransport.AddRange(keysByTcpIp);
            keysAllTransport.AddRange(keysByHttp);
            var diffKeys = keysByExchange.Except(keysAllTransport).ToList();
            if (diffKeys.Count > 0)
            {
                foreach (var diffKey in diffKeys)
                {
                    exceptionStr.AppendFormat("{0}, ", diffKey);
                }
                throw new OptionHandlerException($"Найденно несоответсвие ключей указанных для Обмненов, ключам указанным для транспорта {exceptionStr}");
            }

            //ПРОВЕРКА ОТСУТСВИЯ ДОБАВЛЯЕМОГО ТРАНСПОРТА ДЛЯ КАЖДОГО ОБМЕНА (по ключу KeyTransport). Добавялем только уникальный обмен.
            foreach (var exchangeOption in exchangeOptions)
            {
                if (IsExistTransport(exchangeOption.KeyTransport))
                {
                    exceptionStr.AppendFormat("{0}, ", exchangeOption.KeyTransport);
                }
            }
            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new OptionHandlerException($"Для ExchangeOption УЖЕ СУЩЕСТВУЕТ ТАКОЙ ТРАНСПОРТ:  {exceptionStr}");
            }

            //ДОБАВИТЬ ДЕВАЙС, ОБМЕНЫ, ТРАНСПОРТ
            _deviceOptionRep.Add(deviceOption);
            _exchangeOptionRep.AddRange(exchangeOptions);
            if (transportOption.SerialOptions != null)
                _serialPortOptionRep.AddRange(transportOption.SerialOptions);
            if (transportOption.TcpIpOptions != null)
                _tcpIpOptionRep.AddRange(transportOption.TcpIpOptions);
            if (transportOption.HttpOptions != null)
                _httpOptionRep.AddRange(transportOption.HttpOptions);
        }


        /// <summary>
        /// Проверка наличия транспорта по ключу
        /// </summary>
        private bool IsExistDevice(string deviceName)
        {
            return _deviceOptionRep.IsExist(dev => dev.Name == deviceName);
        }


        /// <summary>
        /// Проверка наличия транспорта по ключу
        /// </summary>
        private bool IsExistExchange(string keyExchange)
        {
            return _exchangeOptionRep.IsExist(exc => exc.Key == keyExchange);
        }


        /// <summary>
        /// Проверка наличия транспорта по ключу
        /// </summary>
        private bool IsExistTransport(KeyTransport keyTransport)
        {
            switch (keyTransport.TransportType)
            {
                case TransportType.SerialPort:
                    return _serialPortOptionRep.IsExist(sp => sp.Port == keyTransport.Key);

                case TransportType.TcpIp:
                    return _tcpIpOptionRep.IsExist(tcpip => tcpip.Name == keyTransport.Key);

                case TransportType.Http:
                    return _httpOptionRep.IsExist(http => http.Name == keyTransport.Key);
            }

            return false;
        }


        private void RemoveTransport(KeyTransport keyTransport)
        {
            switch (keyTransport.TransportType)
            {
                case TransportType.SerialPort:
                     _serialPortOptionRep.Delete(sp=> sp.Port == keyTransport.Key);
                    break;

                case TransportType.TcpIp:
                     _tcpIpOptionRep.Delete(tcpip => tcpip.Name == keyTransport.Key);
                    break;

                case TransportType.Http:
                     _httpOptionRep.Delete(http => http.Name == keyTransport.Key);
                    break;
            }
        }

        #endregion
    }
}