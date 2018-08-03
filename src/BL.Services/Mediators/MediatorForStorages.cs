using System.Collections.Generic;
using System.Linq;
using BL.Services.Storages;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using Exchange.MasterSerialPort;
using Infrastructure.EventBus.Abstract;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Concrete.SpWin;
using Worker.Background.Concrete.HostingBackground;

namespace BL.Services.Mediators
{
    /// <summary>
    /// Сервис объединяет работу со всеми Storage,
    /// и предоставляет интерфейс для Добавления/Удаления элементов в Storage
    /// </summary>
    public class MediatorForStorages
    {
        #region fields

        private readonly DeviceStorageService _deviceStorageService;
        private readonly ExchangeStorageService _exchangeStorageService;
        private readonly BackgroundStorageService _backgroundStorageService;
        private readonly SerialPortStorageService _serialPortStorageService;
        private readonly TcpIpStorageService _tcpIpStorageService;
        private readonly HttpStorageService _httpStorageService;
        private readonly IEventBus _eventBus;

        #endregion




        #region ctor

        public MediatorForStorages(DeviceStorageService deviceStorageService,
            ExchangeStorageService exchangeStorageService,
            BackgroundStorageService backgroundStorageService,
            SerialPortStorageService serialPortStorageService,
            TcpIpStorageService tcpIpStorageService,
            HttpStorageService httpStorageService,
            IEventBus eventBus)
        {
            _serialPortStorageService = serialPortStorageService;
            _tcpIpStorageService = tcpIpStorageService;
            _httpStorageService = httpStorageService;
            _backgroundStorageService = backgroundStorageService;
            _exchangeStorageService = exchangeStorageService;
            _deviceStorageService = deviceStorageService;
            _eventBus = eventBus;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Добавить устройство, которое использует существующий список Exchanges.
        /// </summary>
        public void AddDevice(DeviceOption deviceOption)
        {
            var excanges = _exchangeStorageService.GetMany(deviceOption.ExchangeKeys).ToList();
            var device = new Device.Base.Device(deviceOption, excanges, _eventBus);
            _deviceStorageService.AddNew(deviceOption.Id, device);
        }


        /// <summary>
        /// Добавить устройство, и создать для него список Exchanges, которые используют существующий транспорт
        /// </summary>
        public void AddDevice(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions)
        {
            foreach (var exchOption in exchangeOptions)
            {
                var keyTransport = exchOption.KeyTransport;
                var sp = _serialPortStorageService.Get(keyTransport);
                var bg = _backgroundStorageService.Get(keyTransport);
                if (sp == null || bg == null) continue;
                var key = exchOption.Key;
                var exch = new ByRulesExchangeSerialPort(sp, bg, exchOption);
                _exchangeStorageService.AddNew(key, exch);
            }
        }


        //public void AddDevice(DeviceOption deviceOption,
        //                      ExchangeOption exchangeOption,
        //                      SerialOption serialOption = null,
        //                      TcpIpOption tcpIpOption = null,
        //                      HttpOption httpOption = null)
        //{

        //}


        /// <summary>
        /// Удалить устройство, если список Exchanges не пуст, то удалить все неиспользуемые Exchanges.
        /// </summary>
        public void RemoveDevice(DeviceOption deviceOption)
        {
        }

        #endregion
    }
}


//var env = scope.Resolve<IHostingEnvironment>();
//var serialPortOptionRepository = scope.Resolve<ISerialPortOptionRepository>();
//var exchangeOptionRepository = scope.Resolve<IExchangeOptionRepository>();
//var deviceOptionRepository = scope.Resolve<IDeviceOptionRepository>();

//var serialPortStorageService = scope.Resolve<SerialPortStorageService>();
//var backgroundStorageService = scope.Resolve<BackgroundStorageService>();
//var exchangeStorageService = scope.Resolve<ExchangeStorageService>();
//var deviceStorageService = scope.Resolve<DeviceStorageService>();
//var eventBus = scope.Resolve<IEventBus>();