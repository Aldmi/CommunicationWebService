using System.Threading;
using System.Threading.Tasks;
using BL.Services.Exceptions;
using BL.Services.Storages;


namespace BL.Services.Actions
{
    /// <summary>
    /// Сервис для работы с текущим набором устройств
    /// * Запуск/Останов цикл обмена. 
    /// * Запуск/Останов бекгроунда транспорта. 
    /// </summary>
    public class DeviceActionService
    {
        #region fields

        private readonly DeviceStorageService _deviceStorageService;
        private readonly ExchangeStorageService _exchangeStorageService;
        private readonly BackgroundStorageService _backgroundStorageService;

        #endregion





        #region ctor

        public DeviceActionService(DeviceStorageService deviceStorageService,
            ExchangeStorageService exchangeStorageService,
            BackgroundStorageService backgroundStorageService)
        {
            _deviceStorageService = deviceStorageService;
            _exchangeStorageService = exchangeStorageService;
            _backgroundStorageService = backgroundStorageService;
        }

        #endregion





        #region Methode

        /// <summary>
        /// Установить функции циклического обмена на бекгроунд
        /// </summary>
        /// <param name="exchnageKey"></param>
        public void StartCycleExchange(string exchnageKey)
        {
            var exchange = _exchangeStorageService.Get(exchnageKey);
            if (exchange == null)
                throw new ActionHandlerException($"Обмнена с таким ключем Не найденно: {exchnageKey}");

            if (exchange.IsStartedCycleExchange)
                throw new ActionHandlerException($"Цикл. обмен уже запущенн: {exchnageKey}");

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
                throw new ActionHandlerException($"Обмнена с таким ключем Не найденно: {exchnageKey}");

            if (!exchange.IsStartedCycleExchange)
                throw new ActionHandlerException($"Цикл. обмен уже остановленн: {exchnageKey}");

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
                throw new ActionHandlerException($"ОБмнена с таким ключем Не найденно: {exchnageKey}");

            var bg = _backgroundStorageService.Get(exchange.KeyTransport);
            if (bg.IsStarted)
                throw new ActionHandlerException($"Бекгроунд уже запущен: {bg.KeyTransport}");

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
                throw new ActionHandlerException($"ОБмнена с таким ключем Не найденно: {exchnageKey}");

            var bg = _backgroundStorageService.Get(exchange.KeyTransport);
            if (!bg.IsStarted)
                throw new ActionHandlerException($"Бекгроунд и так остановленн: {bg.KeyTransport}");

            await bg.StopAsync(CancellationToken.None);
        }

        #endregion
    }
}