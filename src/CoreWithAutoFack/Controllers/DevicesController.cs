using System;
using System.Threading.Tasks;
using BL.Services.Actions;
using BL.Services.Mediators;
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

        private readonly MediatorForStorages _mediatorForStorages;
        private readonly DeviceActionService _deviceActionService;

        #endregion




        #region ctor

        public DevicesController(MediatorForStorages mediatorForStorages, DeviceActionService deviceActionService)
        {
            _mediatorForStorages = mediatorForStorages;
            _deviceActionService = deviceActionService;
        }

        #endregion




        #region Methode

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


        //TODO: Добавить Start/Stop exchnage

        #endregion
    }
}