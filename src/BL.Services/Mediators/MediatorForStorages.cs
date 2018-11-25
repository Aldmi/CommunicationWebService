using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Autofac.Features.OwnedInstances;
using BL.Services.Config;
using BL.Services.Exceptions;
using BL.Services.Storages;
using DAL.Abstract.Entities.Options;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using DeviceForExchange;
using Exchange.Base;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using Infrastructure.EventBus.Abstract;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Options;
using Serilog;
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
    public class MediatorForStorages<TIn>
    {
        #region fields

        private readonly DeviceStorageService<TIn> _deviceStorageService;
        private readonly ExchangeStorageService<TIn> _exchangeStorageService;
        private readonly BackgroundStorageService _backgroundStorageService;
        private readonly TransportStorageService _transportStorageService;
        private readonly IEventBus _eventBus;
        private readonly IIndex<string, Func<ProviderOption, IExchangeDataProvider<TIn, ResponseDataItem<TIn>>>> _dataProviderFactory;
        private readonly Func<ProduserOption, Owned<IProduser>> _produser4DeviceRespFactory;

        private readonly AppConfigWrapper _appConfigWrapper;
        private readonly ILogger _logger;
        //опции для создания IProduser через фабрику

        #endregion




        #region ctor

        public MediatorForStorages(DeviceStorageService<TIn> deviceStorageService,
            ExchangeStorageService<TIn> exchangeStorageService,
            BackgroundStorageService backgroundStorageService,
            TransportStorageService transportStorageService,
            IEventBus eventBus,     
            IIndex<string, Func<ProviderOption,IExchangeDataProvider<TIn,ResponseDataItem<TIn>>>> dataProviderFactory,
            Func<ProduserOption, Owned<IProduser>> produser4DeviceRespFactory,
            AppConfigWrapper appConfigWrapper,
            ILogger logger)
        {
            _transportStorageService = transportStorageService;
            _backgroundStorageService = backgroundStorageService;
            _exchangeStorageService = exchangeStorageService;
            _deviceStorageService = deviceStorageService;
            _eventBus = eventBus;
            _dataProviderFactory = dataProviderFactory;
            _produser4DeviceRespFactory = produser4DeviceRespFactory;
            _appConfigWrapper = appConfigWrapper;
            _logger = logger;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Вернуть устройство по  имени устройства.
        /// </summary>
        /// <param name="deviceName">Имя ус-ва, он же ключ к хранилищу</param>
        /// <returns>Верунть ус-во</returns>
        public Device<TIn> GetDevice(string deviceName)
        {
            var device = _deviceStorageService.Get(deviceName);
            return device;
        }


        public IEnumerable<Device<TIn>> GetDevices()
        {
            return _deviceStorageService.Values;
        }


        /// <summary>
        /// Получить список устройств, использующих этот обмен по ключу
        /// </summary>
        /// <param name="exchnageKey"></param>
        /// <returns></returns>
        public IEnumerable<Device<TIn>> GetDevicesUsingExchange(string exchnageKey)
        {
            return _deviceStorageService.Values.Where(dev=>dev.Option.ExchangeKeys.Contains(exchnageKey));
        }


        public IExchange<TIn> GetExchange(string exchnageKey)
        {
            return _exchangeStorageService.Get(exchnageKey);
        }


        public ITransportBackground GetBackground(KeyTransport keyTransport)
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
        public Device<TIn> BuildAndAddDevice(OptionAgregator optionAgregator)
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
                var sp = _transportStorageService.Get(keyTransport);
                if (sp == null)
                {
                    sp = new SpWinSystemIo(spOption, keyTransport);
                    _transportStorageService.AddNew(keyTransport, sp);
                    var bg = new HostingBackgroundTransport(keyTransport, spOption.AutoStart);
                    _backgroundStorageService.AddNew(keyTransport, bg);
                }
            }
            foreach (var tcpIpOption in optionAgregator.TransportOptions.TcpIpOptions)
            {
                var keyTransport = new KeyTransport(tcpIpOption.Name, TransportType.TcpIp);
                var tcpIp = _transportStorageService.Get(keyTransport);
                if (tcpIp == null)
                {
                    tcpIp = new TcpIpTransport(tcpIpOption, keyTransport, _logger);
                    _transportStorageService.AddNew(keyTransport, tcpIp);
                    var bg = new HostingBackgroundTransport(keyTransport, tcpIpOption.AutoStart);
                    _backgroundStorageService.AddNew(keyTransport, bg);
                }
            }
            foreach (var httpOption in optionAgregator.TransportOptions.HttpOptions)
            {
                var keyTransport = new KeyTransport(httpOption.Name, TransportType.Http);
                var http = _transportStorageService.Get(keyTransport);
                if (http == null)
                {
                    http = new HttpTransport(httpOption, keyTransport);
                    _transportStorageService.AddNew(keyTransport, http);
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
                var transport = _transportStorageService.Get(keyTransport);

                try
                {
                    var dataProvider = _dataProviderFactory[exchOption.Provider.Name](exchOption.Provider);
                    exch = new ExchangeUniversal<TIn>(exchOption, transport, bg, dataProvider, _logger);
                    _exchangeStorageService.AddNew(exchOption.Key, exch);
                }
                catch (Exception)
                {
                    throw new StorageHandlerException($"Провайдер данных не найденн в системе: {exchOption.Provider.Name}");
                } 
            }

            //ДОБАВИТЬ УСТРОЙСТВО--------------------------------------------------------------------------
            var excanges = _exchangeStorageService.GetMany(deviceOption.ExchangeKeys).ToList();
            var device = new Device<TIn>(deviceOption, excanges, _eventBus, _produser4DeviceRespFactory, _appConfigWrapper.GetProduser4DeviceOption, _logger);
            _deviceStorageService.AddNew(device.Option.Name, device);

            return device;
        }



        /// <summary>
        /// Удалить устройство,
        /// Если использовались уникальные обмены, то удалить и их. 
        /// Если удаленный (уникальный) обмен использовал уникальный транспорт, то отсановить обмен и удалить транспорт.
        /// </summary>
        public async Task<Device<TIn>> RemoveDevice(string deviceName)
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

            var transport = _transportStorageService.Get(keyTransport);
            if (transport.IsCycleReopened)
            {
                transport.CycleReOpenedCancelation();
            }
            _transportStorageService.Remove(keyTransport);
            transport.Dispose();
        }

        #endregion
    }

}