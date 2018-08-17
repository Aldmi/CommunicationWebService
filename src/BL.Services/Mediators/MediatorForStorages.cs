using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.Services.Exceptions;
using BL.Services.Storages;
using DAL.Abstract.Entities.Options;
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
using Worker.Background.Abstarct;
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
        /// Вернуть устройство по  имени устройства.
        /// </summary>
        /// <param name="deviceName">Имя ус-ва, он же ключ к хранилищу</param>
        /// <returns>Верунть ус-во</returns>
        public Device GetDevice(string deviceName)
        {
            var device = _deviceStorageService.Get(deviceName);
            return device;
        }


        public IEnumerable<Device> GetDevices()
        {
            return _deviceStorageService.Values;
        }


        /// <summary>
        /// Получить список устройств, использующих этот обмен по ключу
        /// </summary>
        /// <param name="exchnageKey"></param>
        /// <returns></returns>
        public IEnumerable<Device> GetDevicesUsingExchange(string exchnageKey)
        {
            return _deviceStorageService.Values.Where(dev=>dev.Option.ExchangeKeys.Contains(exchnageKey));
        }


        public IExchange GetExchange(string exchnageKey)
        {
            return _exchangeStorageService.Get(exchnageKey);
        }


        public IBackground GetBackground(KeyTransport keyTransport)
        {
            return _backgroundStorageService.Get(keyTransport);
        }


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
                    var bg = new HostingBackgroundTransport(keyTransport, spOption.AutoStart);
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
                    var bg = new HostingBackgroundTransport(keyTransport, tcpIpOption.AutoStart);
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
                    var bg = new HostingBackgroundTransport(keyTransport, httpOption.AutoStart);
                    _backgroundStorageService.AddNew(keyTransport, bg);
                }
            }

            //ДОБАВИТЬ НОВЫЕ ОБМЕНЫ---------------------------------------------------------------------------
            foreach (var exchOption in optionAgregator.ExchangeOptions)
            {
                var exch = _exchangeStorageService.Get(exchOption.Key);
                if (exch != null)
                    continue;

                var keyTransport = exchOption.KeyTransport;
                var bg = _backgroundStorageService.Get(keyTransport);
                switch (keyTransport.TransportType)
                {
                    case TransportType.SerialPort:
                        var sp = _serialPortStorageService.Get(keyTransport);
                        exch = new ByRulesExchangeSerialPort(sp, bg, exchOption);
                        _exchangeStorageService.AddNew(exchOption.Key, exch);
                        break;

                    case TransportType.TcpIp:
                        var tcpIp = _tcpIpStorageService.Get(keyTransport);
                        exch = new BaseExchangeTcpIp(tcpIp, bg, exchOption);
                        _exchangeStorageService.AddNew(exchOption.Key, exch);
                        break;

                    case TransportType.Http:
                        var http = _httpStorageService.Get(keyTransport);
                        exch = new BaseExchangeHttp(http, bg, exchOption);
                        _exchangeStorageService.AddNew(exchOption.Key, exch);
                        break;
                }
            }

            //ДОБАВИТЬ УСТРОЙСТВО--------------------------------------------------------------------------
            var excanges = _exchangeStorageService.GetMany(deviceOption.ExchangeKeys).ToList();
            var device = new Device(deviceOption, excanges, _eventBus);
            _deviceStorageService.AddNew(device.Option.Name, device);

            return device;
        }



        /// <summary>
        /// Удалить устройство,
        /// Если использовались уникальные обмены, то удалить и их. 
        /// Если удаленный (уникальный) обмен использовал уникальный транспорт, то отсановить обмен и удалить транспорт.
        /// </summary>
        public async Task<Device> RemoveDevice(string deviceName)
        {
            var device = GetDevice(deviceName);
            if (device == null)
                throw new StorageHandlerException($"Устройство с таким именем НЕ существует: {deviceName}");

            var exchangeKeys = _deviceStorageService.Values.SelectMany(dev => dev.Option.ExchangeKeys).ToList();
            var keyTransports = _exchangeStorageService.Values.Select(exc => exc.KeyTransport).ToList();
            foreach (var exchKey in device.Option.ExchangeKeys)
            {
                if (exchangeKeys.Count(key => key == exchKey) == 1)
                {
                    var removingExch = _exchangeStorageService.Get(exchKey);
                    if (removingExch.IsStartedCycleExchange)
                    {
                        removingExch.StopCycleExchange();
                    }
                    if (keyTransports.Count(tr => tr == removingExch.KeyTransport) == 1)
                    {
                        await RemoveAndStopTransport(removingExch.KeyTransport);
                    }
                    _exchangeStorageService.Remove(exchKey);
                    removingExch.Dispose();
                }
            }

            //УДАЛИМ УСТРОЙСТВО
            _deviceStorageService.Remove(deviceName);
            device.Dispose(); //???
            return device;
        }


        /// <summary>
        /// Ищет транспорт по ключу в нужном хранилище и Удаляет его.
        /// </summary>
        private async Task RemoveAndStopTransport(KeyTransport keyTransport)
        {
            var bg = _backgroundStorageService.Get(keyTransport);
            if (bg.IsStarted)
            {
                _backgroundStorageService.Remove(keyTransport);
                await bg.StopAsync(CancellationToken.None);
                bg.Dispose();
            }

            switch (keyTransport.TransportType)
            {
                case TransportType.SerialPort:
                    var sp = _serialPortStorageService.Get(keyTransport);
                    _serialPortStorageService.Remove(keyTransport);
                    sp.Dispose();
                    break;

                case TransportType.TcpIp:
                    var tcpIp = _tcpIpStorageService.Get(keyTransport);
                    _tcpIpStorageService.Remove(keyTransport);
                    tcpIp.Dispose();
                    break;

                case TransportType.Http:
                    var http = _httpStorageService.Get(keyTransport);
                    _httpStorageService.Remove(keyTransport);
                    http.Dispose();
                    break;
            }
        }

        #endregion



    }
}