using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Services.Exceptions;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Enums;
using Shared.Types;

namespace BL.Services.Mediators
{
    /// <summary>
    /// Сервис объединяет работу с репозиотриями опций для устройств.
    /// DeviceOption + ExchangeOption + TransportOption.
    /// 
    /// </summary>
    public class MediatorForOptions
    {
        #region fields

        private readonly IDeviceOptionRepository _deviceOptionRep;
        private readonly IExchangeOptionRepository _exchangeOptionRep;
        private readonly ISerialPortOptionRepository _serialPortOptionRep;
        private readonly ITcpIpOptionRepository _tcpIpOptionRep;
        private readonly IHttpOptionRepository _httpOptionRep;

        #endregion




        #region ctor

        public MediatorForOptions(IDeviceOptionRepository deviceOptionRep,
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
        public async Task<IEnumerable<DeviceOption>> GetDeviceOptionsAsync()
        {
            return await _deviceOptionRep.ListAsync();
        }

        
        /// <summary>
        /// Вернуть все опции устройств из репозитория.
        /// </summary>
        public async Task<IEnumerable<DeviceOption>> GetDeviceOptionsWithAutoBuildAsync()
        {     
            return await _deviceOptionRep.ListAsync(option=>option.AutoBuild);
        }


        /// <summary>
        /// Вернуть опции одного устройства по имени, из репозитория.
        /// </summary>
        public async Task<DeviceOption> GetDeviceOptionByNameAsync(string deviceName)
        {
            var deviceOption = await _deviceOptionRep.GetSingleAsync(option => option.Name == deviceName);
            return deviceOption;
        }


        /// <summary>
        /// Вернуть все опции обменов из репозитория.
        /// </summary>
        public async Task<IEnumerable<ExchangeOption>> GetExchangeOptionsAsync()
        {
            return await _exchangeOptionRep.ListAsync();
        }


        /// <summary>
        /// Вернуть опции одного обмена из репозитория.
        /// </summary>
        public async Task<ExchangeOption> GetExchangeByKeyAsync(string exchangeKey)
        {
            return await _exchangeOptionRep.GetSingleAsync(option => option.Key == exchangeKey);
        }


        /// <summary>
        /// Вернуть опции всех транспоротов из репозиториев.
        /// </summary>
        public async Task<TransportOption> GetTransportOptionsAsync()
        {
            return new TransportOption
            {
                SerialOptions = (await _serialPortOptionRep.ListAsync()).ToList(),
                TcpIpOptions = (await _tcpIpOptionRep.ListAsync()).ToList(),
                HttpOptions = (await _httpOptionRep.ListAsync()).ToList()
            };
        }


        /// <summary>
        /// Вернуть опции для списка транспортов, по списку ключей из репозитория.
        /// </summary>
        public async Task<TransportOption> GetTransportByKeysAsync(IEnumerable<KeyTransport> keyTransports)
        {
            var serialOptions = new List<SerialOption>();
            var tcpIpOptions = new List<TcpIpOption>();
            var httpOptions = new List<HttpOption>();

            foreach (var keyTransport in keyTransports)
            {
                switch (keyTransport.TransportType)
                {
                    case TransportType.SerialPort:
                        serialOptions.Add(await _serialPortOptionRep.GetSingleAsync(option => option.Port == keyTransport.Key));
                        break;
                    case TransportType.TcpIp:
                        tcpIpOptions.Add(await _tcpIpOptionRep.GetSingleAsync(option => option.Name == keyTransport.Key));
                        break;
                    case TransportType.Http:
                        httpOptions.Add(await _httpOptionRep.GetSingleAsync(option => option.Name == keyTransport.Key));
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
        /// Если указанны все аргументы, то добавляется устройство со спсиком новых обменом и каждый обмен использует новый транспорт.
        /// </summary>
        public async Task<bool> AddDeviceOptionAsync(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions = null, TransportOption transportOption = null)
        {
            if (deviceOption == null && exchangeOptions == null && transportOption == null)
                return false;

            if (deviceOption != null && exchangeOptions == null && transportOption == null)
            {
               await AddDeviceOptionWithExiststExchangeOptionsAsync(deviceOption);
            }
            else
            if (deviceOption != null && exchangeOptions != null && transportOption == null)
            {
               await AddDeviceOptionWithNewExchangeOptionsAsync(deviceOption, exchangeOptions);
            }
            else
            if (deviceOption != null && exchangeOptions != null)
            {
               await AddDeviceOptionWithNewExchangeOptionsAndNewTransportOptionsAsync(deviceOption, exchangeOptions, transportOption);
            }

            return true;
        }


        /// <summary>
        /// Удалим устройство.
        /// Если ключ обмена уникален, то удалим обмен, если в удаляемом обмене уникальный транспорт, то удалим транспорт.
        /// </summary>
        /// <returns>Возвращает все удаленные объекты (Device + Exchanges + Transport)</returns>
        public async Task<OptionAgregator> RemoveDeviceOptionAsync(DeviceOption deviceOption)
        {  
            var deletedOptions= new OptionAgregator
            {
                ExchangeOptions = new List<ExchangeOption>(),
                DeviceOptions = new List<DeviceOption>(),
                TransportOptions = new TransportOption
                {
                    TcpIpOptions = new List<TcpIpOption>(),
                    SerialOptions = new List<SerialOption>(),
                    HttpOptions = new List<HttpOption>()
                }
            };

            //ПРОВЕРКА УНИКАЛЬНОСТИ ОБМЕНОВ УДАЛЯЕМОГО УСТРОЙСТВА (ЕСЛИ УНИКАЛЬНО ТО УДАЛЯЕМ И ОБМЕН, ЕСЛИ ОБМЕН ИСПОЛЬУЕТ УНИКАЛЬНЫЙ ТРАНСПОРТ, ТО УДАЛЯЕМ И ТРАНСПОРТ)
            var exchangeKeys= (await _deviceOptionRep.ListAsync()).SelectMany(option=> option.ExchangeKeys).ToList(); //ключи обменов со всех устройств.
            foreach (var exchangeKey in deviceOption.ExchangeKeys)
            {             
                if (exchangeKeys.Count(key=> key == exchangeKey) == 1)                                                                          //найден обмен используемый только этим устройством
                {
                   var singleExchOption= await _exchangeOptionRep.GetSingleAsync(exc=> exc.Key == exchangeKey);             
                   if ((await _exchangeOptionRep.ListAsync()).Count(option => option.KeyTransport.Equals(singleExchOption.KeyTransport)) == 1)  //найденн транспорт используемый только этим (удаленным) обменом
                   {
                       await RemoveTransportAsync(singleExchOption.KeyTransport, deletedOptions.TransportOptions);                                                               //Удалить транспорт
                   }
                    deletedOptions.ExchangeOptions.Add(singleExchOption);
                    await _exchangeOptionRep.DeleteAsync(singleExchOption);                                                                      //Удалить обмен
                }
            }

            //УДАЛИМ УСТРОЙСТВО
            deletedOptions.DeviceOptions.Add(deviceOption);
            await _deviceOptionRep.DeleteAsync(deviceOption);
            return deletedOptions;
        }


        /// <summary>
        /// Найти Device и все его зависимости в репоизиториях.
        /// Вернуть найденное под OptionAgregator.
        /// </summary>
        public async Task<OptionAgregator> GetOptionAgregatorForDeviceAsync(string deviceName)
        {
            var deviceOption = await GetDeviceOptionByNameAsync(deviceName);
            if (deviceOption == null)
                throw new OptionHandlerException($"Устройство с таким именем Не найденно:  {deviceName}");

            var exchangesOptions= deviceOption.ExchangeKeys.Select(exchangeKey => GetExchangeByKeyAsync(exchangeKey).GetAwaiter().GetResult()).ToList();
            var transportOption= await GetTransportByKeysAsync(exchangesOptions.Select(option => option.KeyTransport).Distinct());
            var optionAgregator = new OptionAgregator 
            {
                DeviceOptions = new List<DeviceOption>{deviceOption},
                ExchangeOptions = exchangesOptions,
                TransportOptions = transportOption
            };
            return optionAgregator;
        }



        /// <summary>
        /// Добавить девайс, который использует уже существующие обмены.
        /// Если хотябы 1 обмен из списка не найденн, то выкидываем Exception.
        /// </summary>
        private async Task AddDeviceOptionWithExiststExchangeOptionsAsync(DeviceOption deviceOption)
        {
            //ПРОВЕРКА ОТСУТСВИЯ УСТРОЙСТВА по имени
            if (await IsExistDeviceAsync(deviceOption.Name))
            {
                throw new OptionHandlerException($"Устройство с таким именем уже существует:  {deviceOption.Name}");
            }

            var exceptionStr = new StringBuilder();
            foreach (var exchangeKey in deviceOption.ExchangeKeys)
            {
                if (! await _exchangeOptionRep.IsExistAsync(exchangeOption => exchangeOption.Key == exchangeKey))
                {
                    exceptionStr.AppendFormat("{0}, ", exchangeKey);
                }
            }

            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new OptionHandlerException($"Не найденны exchangeKeys:  {exceptionStr}");
            }

            await _deviceOptionRep.AddAsync(deviceOption);
        }


        /// <summary>
        /// Добавить девайс, для которого нужно создать 1 или несколько обменов.
        /// Если хотябы для 1 обмена из списка "exchangeOption", не найденн транспорт, то выкидываем Exception.
        /// Если обмен не существует, добавим его, если существует, то игнорируем добавление.
        /// </summary>
        private async Task AddDeviceOptionWithNewExchangeOptionsAsync(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions)
        {
            //ПРОВЕРКА ОТСУТСВИЯ УСТРОЙСТВА по имени
            if (await IsExistDeviceAsync(deviceOption.Name))
            {
                throw new OptionHandlerException($"Устройство с таким именем уже существует:  {deviceOption.Name}");
            }

            var exceptionStr = new StringBuilder();
            var exchangeExternalKeys = exchangeOptions.Select(exchangeOption=> exchangeOption.Key).ToList();

            //ПРОВЕРКА СООТВЕТСТВИЯ exchangeKeys, УКАЗАННОЙ В deviceOption, КЛЮЧАМ ИЗ exchangeOptions
            var diff = exchangeExternalKeys.Except(deviceOption.ExchangeKeys).ToList();
            if (diff.Count > 0)
            {
                throw new OptionHandlerException("Найденно несоответсвие ключей указанных для Device, ключам указанным в exchangeOptions");
            }

            //ПРОВЕРКА НАЛИЧИЯ ОБМЕНОВ УКАЗАННЫХ ДЛЯ УС-ВА В СПИСКЕ НОВЫХ ОБМЕНОВ ИЛИ В СПИСКЕ СУЩЕСТВУЮЩИХ ОБМЕНОВ
            foreach (var exchKey in deviceOption.ExchangeKeys)
            {
               if(exchangeExternalKeys.Contains(exchKey) || _exchangeOptionRep.IsExist(e=> e.Key == exchKey))
                   continue;
                exceptionStr.AppendFormat("{0}, ", exchKey);
            }
            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new OptionHandlerException($"ExchangeKeys указанные для устройства не найденны в списке существующих и добавляемых обменов:  {exceptionStr}");
            }

            //ПРОВЕРКА НАЛИЧИЯ УЖЕ СОЗДАНННОГО ТРАНСПОРТА ДЛЯ КАЖДОГО ОБМЕНА
            foreach (var exchangeOption in exchangeOptions)
            {
                if (! await IsExistTransportAsync(exchangeOption.KeyTransport))
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
                if (! await _exchangeOptionRep.IsExistAsync(exchOpt => exchOpt.Key == exchangeOption.Key))
                {
                    await _exchangeOptionRep.AddAsync(exchangeOption);
                }
            }

            //ДОБАВИТЬ ДЕВАЙС
            await _deviceOptionRep.AddAsync(deviceOption);
        }


        /// <summary>
        /// Добавить девайс, для которого нужно создать 1 или несколько обменов.
        /// Если обмен не существует, добавим его, если существует, то добавим существующий.
        /// Если для нового обменна не существует транспорт, то создадим транспорт.
        /// Если хотябы 1 транспорт в "transportOption" уже существует, то выкидываем Exception.
        /// </summary>
        private async Task AddDeviceOptionWithNewExchangeOptionsAndNewTransportOptionsAsync(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions, TransportOption transportOption)
        {
            //ПРОВЕРКА ОТСУТСВИЯ УСТРОЙСТВА по имени
            if (await IsExistDeviceAsync(deviceOption.Name))
            {
                throw new OptionHandlerException($"Устройство с таким именем уже существует:  {deviceOption.Name}");
            }

            var exceptionStr = new StringBuilder();
            var exchangeExternalKeys = exchangeOptions.Select(exchangeOption => exchangeOption.Key).ToList();

            //ПРОВЕРКА СООТВЕТСТВИЯ exchangeKeys, УКАЗАННОЙ В deviceOption, КЛЮЧАМ ИЗ exchangeOptions
            var diff = exchangeExternalKeys.Except(deviceOption.ExchangeKeys).ToList();
            if (diff.Count > 0)
            {
                throw new OptionHandlerException("Найденно несоответсвие ключей указанных для Device, ключам указанным в exchangeOptions");
            }

            //ПРОВЕРКА НАЛИЧИЯ ОБМЕНОВ УКАЗАННЫХ ДЛЯ УС-ВА В СПИСКЕ НОВЫХ ОБМЕНОВ ИЛИ В СПИСКЕ СУЩЕСТВУЮЩИХ ОБМЕНОВ
            foreach (var exchKey in deviceOption.ExchangeKeys)
            {
                if(exchangeExternalKeys.Contains(exchKey) || _exchangeOptionRep.IsExist(e=> e.Key == exchKey))
                    continue;
                exceptionStr.AppendFormat("{0}, ", exchKey);
            }
            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new OptionHandlerException($"ExchangeKeys указанные для устройства не найденны в списке существующих и добавляемых обменов:  {exceptionStr}");
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
                if (await IsExistTransportAsync(exchangeOption.KeyTransport))
                {
                    exceptionStr.AppendFormat("{0}, ", exchangeOption.KeyTransport);
                }
            }
            if (!string.IsNullOrEmpty(exceptionStr.ToString()))
            {
                throw new OptionHandlerException($"Для ExchangeOption УЖЕ СУЩЕСТВУЕТ ТАКОЙ ТРАНСПОРТ:  {exceptionStr}");
            }

            //ДОБАВИТЬ ДЕВАЙС, ОБМЕНЫ, ТРАНСПОРТ
           await _deviceOptionRep.AddAsync(deviceOption);
           await _exchangeOptionRep.AddRangeAsync(exchangeOptions);
            if (transportOption.SerialOptions != null)
                await _serialPortOptionRep.AddRangeAsync(transportOption.SerialOptions);
            if (transportOption.TcpIpOptions != null)
                await _tcpIpOptionRep.AddRangeAsync(transportOption.TcpIpOptions);
            if (transportOption.HttpOptions != null)
                await _httpOptionRep.AddRangeAsync(transportOption.HttpOptions);
        }


        /// <summary>
        /// Проверка наличия устройтсва по имени
        /// </summary>
        public async Task<bool> IsExistDeviceAsync(string deviceName)
        {
            return await _deviceOptionRep.IsExistAsync(dev => dev.Name == deviceName);
        }


        /// <summary>
        /// Проверка наличия транспорта по ключу
        /// </summary>
        public async Task<bool> IsExistExRRchangeAsync(string keyExchange)
        {
            return await _exchangeOptionRep.IsExistAsync(exc => exc.Key == keyExchange);
        }


        /// <summary>
        /// Проверка наличия транспорта по ключу
        /// </summary>
        public async Task<bool> IsExistTransportAsync(KeyTransport keyTransport)
        {
            switch (keyTransport.TransportType)
            {
                case TransportType.SerialPort:
                    return await _serialPortOptionRep.IsExistAsync(sp => sp.Port == keyTransport.Key);

                case TransportType.TcpIp:
                    return await _tcpIpOptionRep.IsExistAsync(tcpip => tcpip.Name == keyTransport.Key);

                case TransportType.Http:
                    return await _httpOptionRep.IsExistAsync(http => http.Name == keyTransport.Key);
            }

            return false;
        }


        /// <summary>
        /// Ищет транспорт по ключу в нужном репозитории и Удаляет его.
        /// Удаленный транспорт помещается в deletedTransport.
        /// </summary>
        private async Task RemoveTransportAsync(KeyTransport keyTransport, TransportOption deletedTransport)
        {
            switch (keyTransport.TransportType)
            {
                case TransportType.SerialPort:
                    deletedTransport.SerialOptions.Add(await _serialPortOptionRep.GetSingleAsync(sp=> sp.Port == keyTransport.Key));
                    await _serialPortOptionRep.DeleteAsync(sp=> sp.Port == keyTransport.Key);
                    break;

                case TransportType.TcpIp:
                    deletedTransport.TcpIpOptions.Add(await _tcpIpOptionRep.GetSingleAsync(tcpip => tcpip.Name == keyTransport.Key));
                    await _tcpIpOptionRep.DeleteAsync(tcpip=> tcpip.Name == keyTransport.Key);
                    break;

                case TransportType.Http:
                    deletedTransport.HttpOptions.Add(await _httpOptionRep.GetSingleAsync(http => http.Name == keyTransport.Key));
                    await _httpOptionRep.DeleteAsync(http=> http.Name == keyTransport.Key);
                    break;
            }
        }

        #endregion
    }
}