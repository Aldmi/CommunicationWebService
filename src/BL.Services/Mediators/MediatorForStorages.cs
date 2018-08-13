using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.Services.Exceptions;
using BL.Services.Storages;
using DAL.Abstract.Entities.Options;
using DeviceForExchange;
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
        /// <param name="optionAgregator">Цепочка настроек одного устройства. Настройкам этим полностью доверяем (не валидируем).</param>
        /// <returns> Новое созданное ус-во, добавленное в хранилище</returns>
        public Device BuildAndAddDevice(OptionAgregator optionAgregator)
        {
            var deviceOption = optionAgregator.DeviceOptions.First();
            if (_deviceStorageService.IsExist(deviceOption.Name))
            {
                throw new StorageHandlerException($"Устройство с таким именем уже существует: {deviceOption.Name}");
            }

            //ДОБАВИТЬ НОВЫЙ ТРАНСПОРТ-----------------------------------------------------------------------
            foreach (var spOption in optionAgregator.TransportOptions.SerialOptions)
            {
                var keyTransport = new KeyTransport(spOption.Port, TransportType.SerialPort);
                var sp = _serialPortStorageService.Get(keyTransport);
                if (sp == null)
                {
                    sp = new SpWinSystemIo(spOption, keyTransport);        
                    _serialPortStorageService.AddNew(keyTransport, sp);
                    var bg = new HostingBackgroundTransport(keyTransport);
                    _backgroundStorageService.AddNew(keyTransport, bg);
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
                    var bg = new HostingBackgroundTransport(keyTransport);
                    _backgroundStorageService.AddNew(keyTransport, bg);
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
                    var bg = new HostingBackgroundTransport(keyTransport);
                    _backgroundStorageService.AddNew(keyTransport, bg);
                }
            }

            //ДОБАВИТЬ НОВЫЕ ОБМЕНЫ---------------------------------------------------------------------------
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

            //ДОБАВИТЬ УСТРОЙСТВО--------------------------------------------------------------------------
            var excanges= _exchangeStorageService.GetMany(deviceOption.ExchangeKeys).ToList();
            var device= new Device(deviceOption, excanges, _eventBus);
            _deviceStorageService.AddNew(device.Option.Name, device);

            return device;
        }



        /// <summary>
        /// Вернуть устройство по  имени устройства.
        /// </summary>
        /// <param name="deviceName">Имя ус-ва, он же ключ к хранилищу</param>
        /// <returns>Верунть ус-во</returns>
        public Device GetDevice(string deviceName)
        {
            var device= _deviceStorageService.Get(deviceName);
            return device;
        }


        /// <summary>
        /// Удалить устройство, если список Exchanges не пуст, то удалить все неиспользуемые Exchanges.
        /// </summary>
        public DictionaryCrudResult RemoveDevice(string deviceName)
        {
           var result= _deviceStorageService.Remove(deviceName);
           return result;
        }


        /// <summary>
        /// Установить функции циклического обмена на бекгроунд
        /// </summary>
        /// <param name="exchnageKey"></param>
        public void StartCycleExchange(string exchnageKey)
        {
            var exchange = _exchangeStorageService.Get(exchnageKey);
            if (exchange == null)
                throw new OptionHandlerException($"ОБмнена с таким ключем Не найденно: {exchnageKey}");

            if (exchange.IsStartedCycleExchange)
                throw new OptionHandlerException($"Цикл. обмен уже запущенн: {exchnageKey}");

            exchange.StartCycleExchange();
        }


        /// <summary>
        /// Снять функции циклического обмена с бекгроунда
        /// </summary>
        /// <param name="exchnageKey"></param>
        public void StopCycleExchange(string exchnageKey)
        {
            var exchange = _exchangeStorageService.Get(exchnageKey);
            if (exchange == null)
                throw new OptionHandlerException($"Обмнена с таким ключем Не найденно: {exchnageKey}");

            if (!exchange.IsStartedCycleExchange)
                throw new OptionHandlerException($"Цикл. обмен уже остановленн: {exchnageKey}");

            exchange.StopCycleExchange();
        }


        /// <summary>
        /// Запустить Бекграунд обмена 
        /// </summary>
        /// <param name="exchnageKey"></param>
        public async Task StartBackground(string exchnageKey)
        {
            var exchange = _exchangeStorageService.Get(exchnageKey);
            if (exchange == null)
                throw new OptionHandlerException($"ОБмнена с таким ключем Не найденно: {exchnageKey}");

           var bg= _backgroundStorageService.Get(exchange.KeyTransport);
           if(bg.IsStarted)
               throw new OptionHandlerException($"Бекгроунд уже запущен: {bg.KeyTransport}");

            await bg.StartAsync(CancellationToken.None);
        }



        /// <summary>
        /// Остановить Бекграунд обмена 
        /// </summary>
        /// <param name="exchnageKey"></param>
        public async Task StopBackground(string exchnageKey)
        {
            var exchange = _exchangeStorageService.Get(exchnageKey);
            if (exchange == null)
                throw new OptionHandlerException($"ОБмнена с таким ключем Не найденно: {exchnageKey}");

            var bg= _backgroundStorageService.Get(exchange.KeyTransport);
            if (!bg.IsStarted)
                throw new OptionHandlerException($"Бекгроунд и так остановленн: {bg.KeyTransport}");

            await bg.StopAsync(CancellationToken.None);
        }
        #endregion
    }
}