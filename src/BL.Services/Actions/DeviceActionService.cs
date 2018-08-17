using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.Services.Exceptions;
using BL.Services.Mediators;
using BL.Services.Storages;
using Shared.Types;


namespace BL.Services.Actions
{
    /// <summary>
    /// Сервис для работы с текущим набором устройств
    /// * Запуск/Останов цикл обмена. 
    /// * Запуск/Останов бекгроунда транспорта. 
    /// * Послать вручную данные на обмен ус-ва.
    /// 
    /// </summary>
    public class DeviceActionService
    {
        #region fields

        private readonly MediatorForStorages _mediatorForStorages;

        #endregion




        #region ctor

        public DeviceActionService(MediatorForStorages mediatorForStorages)
        {
            _mediatorForStorages = mediatorForStorages;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Установить функции циклического обмена на бекгроунд
        /// </summary>
        /// <param name="exchnageKey"></param>
        public void StartCycleExchange(string exchnageKey)
        {
            
            var exchange = _mediatorForStorages.GetExchange(exchnageKey);
            if (exchange == null)
                throw new ActionHandlerException($"Обмен с таким ключем Не найден: {exchnageKey}");

            if (exchange.IsStartedCycleExchange)
                throw new ActionHandlerException($"Цикл. обмен уже запущен: {exchnageKey}");

            exchange.StartCycleExchange();
        }


        /// <summary>
        /// Снять функции циклического обмена с бекгроунда
        /// </summary>
        /// <param name="exchnageKey"></param>
        public void StopCycleExchange(string exchnageKey)
        {
            var exchange = _mediatorForStorages.GetExchange(exchnageKey);
            if (exchange == null)
                throw new ActionHandlerException($"Обмен с таким ключем Не найден: {exchnageKey}");

            if (!exchange.IsStartedCycleExchange)
                throw new ActionHandlerException($"Цикл. обмен уже остановлен: {exchnageKey}");

            exchange.StopCycleExchange();
        }


        /// <summary>
        /// Установить функции циклических обменов на бекгроунды
        /// </summary>
        /// <param name="exchnageKeys"></param>
        public void StartCycleExchanges(IEnumerable<string> exchnageKeys)
        {
            foreach (var exchnageKey in exchnageKeys)
            {
                StartCycleExchange(exchnageKey);
            }
        }


        /// <summary>
        /// Снять функции циклических обмена с бекгроундов
        /// </summary>
        /// <param name="exchnageKeys"></param>
        public void StopCycleExchanges(IEnumerable<string> exchnageKeys)
        {
            foreach (var exchnageKey in exchnageKeys)
            {
                StopCycleExchange(exchnageKey);
            }
        }


        //БЕКГРОУНД ЗАПУСКАТЬ/ ИЗ ОБМЕНА НЕЛЬЗЯ, Т.К.ОДИН БГ МОЖЕТ ВХОДИТЬ ВО МНОГО ОБМЕНОВ
        /// <summary>
        /// Запустить Бекграунд транспорта. 
        /// </summary>
        /// <param name="keyTransport"></param>
        public async Task StartBackground(KeyTransport keyTransport)
        {
            
            var bg = _mediatorForStorages.GetBackground(keyTransport);
            if (bg.IsStarted)
                throw new ActionHandlerException($"Бекграунд уже запущен: {bg.KeyTransport}");

            await bg.StartAsync(CancellationToken.None);
        }


        /// <summary>
        /// Остановить Бекграунд транспорта 
        /// </summary>
        /// <param name="keyTransport"></param>
        public async Task StopBackground(KeyTransport keyTransport)
        {
            var bg = _mediatorForStorages.GetBackground(keyTransport);
            if (!bg.IsStarted)
                throw new ActionHandlerException($"Бекграунд и так остановлен: {bg.KeyTransport}");

            await bg.StopAsync(CancellationToken.None);
        }


        /// <summary>
        /// Запустить коллекцию бекграундов
        /// </summary>
        /// <param name="keysTransport"></param>
        /// <returns></returns>
        public async Task StartBackgrounds(IEnumerable<KeyTransport> keysTransport)
        {
            var tasks= keysTransport.Select(StartBackground).ToList();
            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Остановить коллекцию бекграундов
        /// </summary>
        /// <param name="keysTransport"></param>
        /// <returns></returns>
        public async Task StopBackgrounds(IEnumerable<KeyTransport> keysTransport)
        {
            var tasks= keysTransport.Select(StopBackground).ToList();
            await Task.WhenAll(tasks);
        }



        /// <summary>
        /// Отправить данные на конкретный обмен.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="exchnageKey"></param>
        /// <returns></returns>
        public async Task SendData(string exchnageKey)//TODO: Добавить данные
        {
            var exch = _mediatorForStorages.GetExchange(exchnageKey);
            if(exch == null)
                throw new ActionHandlerException($"Обмен с таким ключем Не найден: {exchnageKey}");

            //exch.SendOneTimeData();

            await Task.CompletedTask;
        }


        ///// <summary>
        ///// Отправить данные на все обмены ус-ва.
        ///// </summary>
        ///// <param name="deviceName"></param>
        ///// <returns></returns>
        //public async Task SendData(string deviceName)
        //{
        //    await Task.CompletedTask;
        //}

        #endregion
    }
}