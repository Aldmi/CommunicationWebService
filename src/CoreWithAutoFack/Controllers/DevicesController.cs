using System;
using System.Threading.Tasks;
using BL.Services.Actions;
using BL.Services.Exceptions;
using BL.Services.Mediators;
using InputDataModel.Autodictor.Model;
using Microsoft.AspNetCore.Mvc;

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

        #endregion




        #region ctor

        public DevicesController(MediatorForStorages<AdInputType> mediatorForStorages, DeviceActionService<AdInputType> deviceActionService)
        {
            _mediatorForStorages = mediatorForStorages;
            _deviceActionService = deviceActionService;
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                //LOG
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
                Console.WriteLine(ex);
                //LOG
                ModelState.AddModelError("StartCycleExchangeException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //LOG
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
                Console.WriteLine(ex);
                //LOG
                ModelState.AddModelError("StartCycleExchangeException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //LOG
                throw;
            }
        }




        #endregion
    }
}