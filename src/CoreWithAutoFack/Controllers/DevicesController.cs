using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Services.Actions;
using BL.Services.Exceptions;
using BL.Services.Mediators;
using InputDataModel.Autodictor.Model;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebServer.Controllers
{
    /// <summary>
    /// REST api доступа к оперативному хранилищу устройств (Devices, Exchanges, Transports).
    /// Удалить устройство из хранилища, остановить все эксклюзивные сервисы, связанные с ним (Exchanges, Transports)
    /// Start/Stop цикл. обмен на Exchange.
    /// Start/Stop БГ для транспорта.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        #region fields

        private readonly MediatorForStorages<AdInputType> _mediatorForStorages;
        private readonly DeviceActionService<AdInputType> _deviceActionService;
        private readonly ILogger _logger;

        #endregion




        #region ctor

        public DevicesController(MediatorForStorages<AdInputType> mediatorForStorages,
                                 DeviceActionService<AdInputType> deviceActionService,
                                 ILogger logger)
        {
            _mediatorForStorages = mediatorForStorages;
            _deviceActionService = deviceActionService;
            _logger = logger;
        }

        #endregion




        #region Methode

        // GET api/Devices/GetDevices
        [HttpGet("GetDevices")]
        public async Task<IActionResult> GetDevices()
        {
            var devices = _mediatorForStorages.GetDevices();
            await Task.CompletedTask;
            return new JsonResult(devices);
        }


        // GET api/Devices/GetDevicesUsingExchange/exchnageKey
        [HttpGet("{exchnageKey}")]
        public async Task<IActionResult> GetDevicesUsingExchange([FromRoute] string exchnageKey)
        {
            var devicesUsingExchange= _mediatorForStorages.GetDevicesUsingExchange(exchnageKey);
            await Task.CompletedTask;
            return new JsonResult(devicesUsingExchange);
        }


        //TODO: добавить GetDevicesUsingBackground

        //TODO: добавить GetExchangesState (string deviceName)


        [HttpDelete("{deviceName}")]
        public async Task<IActionResult> RemoveDevice([FromRoute] string deviceName)
        {
            var device= _mediatorForStorages.GetDevice(deviceName);
            if (device == null)
            {
                return NotFound(deviceName);
            }

            try
            {
                await _mediatorForStorages.RemoveDevice(deviceName);
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка в DevicesController/RemoveDevice");
                throw;
            }
        }



        /// <summary>
        /// Запустить обмен по ключу
        /// </summary>
        /// <param name="exchnageKey">Список устройств которые используют данный обмен</param>
        /// <returns></returns>
        [HttpPut("StartCycleExchange/{exchnageKey}")]
        public IActionResult StartCycleExchange([FromRoute] string exchnageKey)
        {
            try
            {
                 _deviceActionService.StartCycleExchange(exchnageKey);
                var devicesUsingExchange=  _mediatorForStorages.GetDevicesUsingExchange(exchnageKey);
                return Ok(devicesUsingExchange);
            }
            catch (ActionHandlerException ex)
            {
                _logger.Error(ex, "Ошибка в DevicesController/StartCycleExchange");
                ModelState.AddModelError("StartCycleExchangeException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Критическая Ошибка в DevicesController/StartCycleExchange");
                throw;
            }
        }


        /// <summary>
        /// Остановить обмен по ключу
        /// </summary>
        /// <param name="exchnageKey">Список устройств которые используют данный обмен</param>
        /// <returns></returns>
        [HttpPut("StopCycleExchange/{exchnageKey}")]
        public IActionResult StopCycleExchange([FromRoute] string exchnageKey)
        {
            try
            {
                _deviceActionService.StopCycleExchange(exchnageKey);
                var devicesUsingExchange=  _mediatorForStorages.GetDevicesUsingExchange(exchnageKey);
                return Ok(devicesUsingExchange);
            }
            catch (ActionHandlerException ex)
            {
                _logger.Error(ex, "Ошибка в DevicesController/StopCycleExchange");
                ModelState.AddModelError("StartCycleExchangeException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Критическая Ошибка в DevicesController/StopCycleExchange");
                throw;
            }
        }


        // PUT api/Devices/StartCycleReOpenedConnection
        [HttpPut("StartCycleReOpenedConnection")]
        public async Task<IActionResult> StartCycleReOpenedConnection([FromBody] IEnumerable<string> exchnageKeys)
        {
            try
            {
               await _deviceActionService.StartCycleReOpenedConnections(exchnageKeys);
               return Ok(exchnageKeys);
            }
            catch (ActionHandlerException ex)
            {
                _logger.Error(ex, "Ошибка в DevicesController/StartCycleReOpenedConnection");
                ModelState.AddModelError("StartCycleReOpenedConnection", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Критическая Ошибка в DevicesController/StartCycleReOpenedConnection");
                throw;
            }
        }


        // PUT api/Devices/StartCycleReOpenedConnection
        [HttpPut("StopCycleReOpenedConnection")]
        public IActionResult StopCycleReOpenedConnection([FromBody] IEnumerable<string> exchnageKeys)
        {
            try
            {
                _deviceActionService.StopCycleReOpenedConnections(exchnageKeys);
                return Ok(exchnageKeys);
            }
            catch (ActionHandlerException ex)
            {
                _logger.Error(ex, "Ошибка в DevicesController/StopCycleReOpenedConnection");
                ModelState.AddModelError("StopCycleReOpenedConnections", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Критическая Ошибка в DevicesController/StopCycleReOpenedConnection");
                throw;
            }
        }

        //TODO: добавить StartBackgrounds , StopBackgrounds

        #endregion
    }
}