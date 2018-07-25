using System.Collections.Generic;
using System.Linq;
using BL.Services.Storage;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;
using DAL.Abstract.Entities.Exchange;
using DAL.Abstract.Entities.Transport;
using Exchange.MasterSerialPort;
using Infrastructure.EventBus.Abstract;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Concrete.SpWin;
using Worker.Background.Concrete.HostingBackground;

namespace BL.Services.Editors
{
    /// <summary>
    /// Сервис объединяет все Storage,
    /// и предоставляет интерфейс для Добавления/Удаления элементов в Storage
    /// </summary>
    public class EditorStoragesService
    {
        #region fields

        private readonly SerialPortStorageService _serialPortStorageService;
        private readonly BackgroundStorageService _backgroundStorageService;
        private readonly ExchangeStorageService _exchangeStorageService;
        private readonly DeviceStorageService _deviceStorageService;
        private readonly IEventBus _eventBus;

        #endregion




        #region ctor

        public EditorStoragesService(SerialPortStorageService serialPortStorageService,
            BackgroundStorageService backgroundStorageService,
            ExchangeStorageService exchangeStorageService,
            DeviceStorageService deviceStorageService,
            IEventBus eventBus)
        {
            _serialPortStorageService = serialPortStorageService;
            _backgroundStorageService = backgroundStorageService;
            _exchangeStorageService = exchangeStorageService;
            _deviceStorageService = deviceStorageService;
            _eventBus = eventBus;
        }

        #endregion



        #region Methode

        public void InitializeByRepositoryOption(ISerialPortOptionRepository serialPortOptionRepository,
                                                 IExchangeOptionRepository exchangeOptionRepository,
                                                 IDeviceOptionRepository deviceOptionRepository)
        {
            //ADD SERIAL PORTS--------------------------------------------------------------------
            foreach (var spOption in serialPortOptionRepository.List())
            {
                var keyTransport = new KeyTransport(spOption.Port, TransportType.SerialPort);
                var sp = new SpWinSystemIo(spOption, keyTransport);
                var bg = new HostingBackgroundTransport(keyTransport);
                _serialPortStorageService.AddNew(keyTransport, sp);
                _backgroundStorageService.AddNew(keyTransport, bg);
            }

            //ADD EXCHANGES------------------------------------------------------------------------
            foreach (var exchOption in exchangeOptionRepository.List())
            {
                var keyTransport = exchOption.KeyTransport;
                var sp = _serialPortStorageService.Get(keyTransport);
                var bg = _backgroundStorageService.Get(keyTransport);
                if (sp == null || bg == null) continue;
                var exch = new ByRulesExchangeSerialPort(sp, bg, exchOption);
                _exchangeStorageService.AddNew(keyTransport, exch);
            }

            //ADD DEVICES--------------------------------------------------------------------------
            foreach (var deviceOption in deviceOptionRepository.List())
            {
                AddDevice(deviceOption);
            }
        }


        /// <summary>
        /// Добавить устройство, которое использует существующий список Exchanges.
        /// </summary>
        public void AddDevice(DeviceOption deviceOption)
        {
          var exchanges= _exchangeStorageService.GetMany(deviceOption.KeyTransports).ToList();
          var device = new Device.Base.Device(deviceOption, exchanges, _eventBus);
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
                var exch = new ByRulesExchangeSerialPort(sp, bg, exchOption);
                _exchangeStorageService.AddNew(keyTransport, exch);
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