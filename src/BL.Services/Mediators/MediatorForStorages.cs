using System;
using System.Collections.Generic;
using System.Linq;
using BL.Services.Mediators.Exceptions;
using BL.Services.Storages;
using DAL.Abstract.Entities.Options;
using DAL.Abstract.Entities.Options.Device;
using DeviceForExchange;
using Exchange.Base;
using Exchange.Http;
using Exchange.MasterSerialPort;
using Exchange.TcpIp;
using Infrastructure.EventBus.Abstract;
using Shared.Enums;
using Shared.Types;
using Transport.Http.Concrete;
using Transport.SerialPort.Concrete.SpWin;
using Transport.TcpIp.Concrete;
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
        /// Создать устройство на базе optionAgregator.
        /// Созданное ус-во добавляется в StorageDevice. 
        /// Если для создания ус-ва нужно создать ОБМЕН и/или ТРАНСПОРТ, то созданные объекты тоже добавляются в StorageExchange или StorageTransport
        /// </summary>
        public Device BuildAndAddDevice(OptionAgregator optionAgregator)
        {
            var device = optionAgregator.DeviceOptions.First();
            if (_deviceStorageService.IsExist(device.Name))
            {
                throw new StorageHandlerException($"Устройство с таким именем уже существует: {device.Name}");
            }

            //ДОБАВИТЬ ТРАНСПОРТ-----------------------------------------------------------------------
            foreach (var spOption in optionAgregator.TransportOptions.SerialOptions)
            {
                var keyTransport = new KeyTransport(spOption.Port, TransportType.SerialPort);
                var sp = _serialPortStorageService.Get(keyTransport);
                if (sp == null)
                {
                    sp = new SpWinSystemIo(spOption, keyTransport);
                    _serialPortStorageService.AddNew(keyTransport, sp);
                }
            }
            foreach (var tcpIpOption in optionAgregator.TransportOptions.TcpIpOptions)
            {
                var keyTransport = new KeyTransport(tcpIpOption.Name, TransportType.TcpIp);
                var tcpIp = _tcpIpStorageService.Get(keyTransport);
                if (tcpIp == null)
                {
                    tcpIp = new TcpIpTransport(tcpIpOption, keyTransport);
                    _tcpIpStorageService.AddNew(keyTransport, tcpIp);
                }
            }
            foreach (var httpOption in optionAgregator.TransportOptions.HttpOptions)
            {
                var keyTransport = new KeyTransport(httpOption.Name, TransportType.Http);
                var http = _httpStorageService.Get(keyTransport);
                if (http == null)
                {
                    http = new HttpTransport(httpOption, keyTransport);
                    _httpStorageService.AddNew(keyTransport, http);
                }
            }

            //ДОБАВИТЬ ОБМЕНЫ---------------------------------------------------------------------------
            foreach (var exchOption in  optionAgregator.ExchangeOptions)
            {
                var exch= _exchangeStorageService.Get(exchOption.Key);
                if (exch != null)
                    continue;

                var keyTransport= exchOption.KeyTransport;
                var bg= _backgroundStorageService.Get(keyTransport);
                switch (keyTransport.TransportType)
                {
                    case TransportType.SerialPort:
                        var sp= _serialPortStorageService.Get(keyTransport);                     
                        exch= new ByRulesExchangeSerialPort(sp, bg, exchOption);
                        _exchangeStorageService.AddNew(exchOption.Key, exch);
                        break;

                    case TransportType.TcpIp:
                        var tcpIp= _tcpIpStorageService.Get(keyTransport);
                        exch= new BaseExchangeTcpIp(tcpIp, bg, exchOption);
                        _exchangeStorageService.AddNew(exchOption.Key, exch);
                        break;

                    case TransportType.Http:
                        var http= _httpStorageService.Get(keyTransport);
                        exch= new BaseExchangeHttp(http, bg, exchOption);
                        _exchangeStorageService.AddNew(exchOption.Key, exch);
                        break;
                }
            }

            //ДОБАВИТЬ УСТРОЙСТВА--------------------------------------------------------------------------


            //var exchanges = new List<IExchange>();
            //foreach (var exchOption in  optionAgregator.ExchangeOptions)
            //{
            //    var exch= _exchangeStorageService.Get(exchOption.Key);
            //    //СОЗДАТЬ НОВЫЙ ОБМЕН
            //    if (exch == null)
            //    {
            //        var keyTransport= exchOption.KeyTransport;
            //        var backGround= new HostingBackgroundTransport(keyTransport);
            //        switch (keyTransport.TransportType)
            //        {
            //            case TransportType.SerialPort:                   
            //                var sp= _serialPortStorageService.Get(keyTransport);
            //                if (sp == null)
            //                {
            //                    sp= new SpWinSystemIo(optionAgregator.);
            //                    //СОЗДАТЬ НОВЫЙ ТРАНСПОРТ
            //                }
            //                exch= new ByRulesExchangeSerialPort(sp, backGround, exchOption);
            //                break;

            //            case TransportType.TcpIp:
            //                break;

            //            case TransportType.Http:
            //                break;
            //        }
            //    }
            //    //ДОБАВИТЬ ТЕКУЩИЙ
            //    else
            //    {
            //        exchanges.Add(exch);
            //    }
            //}


            //foreach (var exchangeKey in device.ExchangeKeys)
            //{
            //   var exch= _exchangeStorageService.Get(exchangeKey);
            //   //СОЗДАТЬ НОВЫЙ ОБМЕН
            //   if (exch == null)
            //   {
            //       var keyTransport= exchOption.KeyTransport;
            //   }
            //   //ДОБАВИТЬ ТЕКУЩИЙ
            //   else
            //   {
            //       exchanges.Add(exch);
            //   }
            //}

            return null;

            //var excanges = _exchangeStorageService.GetMany(deviceOption.ExchangeKeys).ToList();
            //var device = new Device.Base.Device(deviceOption, excanges, _eventBus);
            //_deviceStorageService.AddNew(deviceOption.Id, device);
        }


        /// <summary>
        /// Добавить устройство, и создать для него список Exchanges, которые используют существующий транспорт
        /// </summary>
        //public void AddDevice(DeviceOption deviceOption, IEnumerable<ExchangeOption> exchangeOptions)
        //{
        //    foreach (var exchOption in exchangeOptions)
        //    {
        //        var keyTransport = exchOption.KeyTransport;
        //        var sp = _serialPortStorageService.Get(keyTransport);
        //        var bg = _backgroundStorageService.Get(keyTransport);
        //        if (sp == null || bg == null) continue;
        //        var key = exchOption.Key;
        //        var exch = new ByRulesExchangeSerialPort(sp, bg, exchOption);
        //        _exchangeStorageService.AddNew(key, exch);
        //    }
        //}


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