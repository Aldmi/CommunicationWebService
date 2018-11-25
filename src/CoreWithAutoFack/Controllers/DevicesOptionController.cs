using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BL.Services.Actions;
using BL.Services.Exceptions;
using BL.Services.Mediators;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Transport;
using InputDataModel.Autodictor.Model;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebServer.DTO.JSON.OptionsDto;
using WebServer.DTO.JSON.OptionsDto.DeviceOption;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption;
using WebServer.DTO.JSON.OptionsDto.TransportOption;

namespace WebServer.Controllers
{
    /// <summary>
    /// REST api доступа к опциям системы (Devices, Exchanges, Transports)
    /// На базе опций можно сбилдить Device и сохранить его в Storage
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DevicesOptionController : Controller
    {
        #region fields

        private readonly MediatorForOptions _mediatorForOptionsRep;
        private readonly BuildDeviceService<AdInputType> _buildDeviceService;

        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        #endregion




        #region ctor

        public DevicesOptionController(MediatorForOptions mediatorForOptionsRep,
                                       BuildDeviceService<AdInputType> buildDeviceService,
                                       IMapper mapper,
                                       ILogger logger)
        {
            _mediatorForOptionsRep = mediatorForOptionsRep;
            _buildDeviceService = buildDeviceService;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion




        #region ApiMethode

        // GET api/devicesoption
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var deviceOptions = await _mediatorForOptionsRep.GetDeviceOptionsAsync();
                var exchangeOptions = await _mediatorForOptionsRep.GetExchangeOptionsAsync();
                var transportOption = await _mediatorForOptionsRep.GetTransportOptionsAsync();

                var deviceOptionsDto = _mapper.Map<List<DeviceOptionDto>>(deviceOptions);
                var exchangeOptionsDto = _mapper.Map<List<ExchangeOptionDto>>(exchangeOptions);
                var transportOptionDto = _mapper.Map<TransportOptionsDto>(transportOption);

                var agregatorOptionDto = new OptionAgregatorDto
                {
                    DeviceOptions = deviceOptionsDto,
                    ExchangeOptions = exchangeOptionsDto,
                    TransportOptions = transportOptionDto
                };
                return new JsonResult(agregatorOptionDto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка в DevicesOptionController/Get");
                throw;
            }
        }



        // GET api/devicesoption/deviceName
        [HttpGet("{deviceName}", Name = "GetDevice")]
        public async Task<IActionResult> Get([FromRoute]string deviceName)
        {
            try
            {
                if (!await _mediatorForOptionsRep.IsExistDeviceAsync(deviceName))
                {
                    return NotFound(deviceName);
                }
                var optionAgregator = await _mediatorForOptionsRep.GetOptionAgregatorForDeviceAsync(deviceName);
                var agregatorOptionDto = _mapper.Map<OptionAgregatorDto>(optionAgregator);
                return new JsonResult(agregatorOptionDto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка в DevicesOptionController/Get");
                throw;
            }
        }



        // POST api/devicesoption
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OptionAgregatorDto data)
        {
            if (data == null)
            {
                ModelState.AddModelError("AgregatorOptionDto", "POST body is null");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var deviceOptionDto = data.DeviceOptions?.FirstOrDefault();
                var exchangeOptionDto = data.ExchangeOptions;
                var transportOptionDto = data.TransportOptions;
                var deviceOption = _mapper.Map<DeviceOption>(deviceOptionDto);
                var exchangeOption = _mapper.Map<IEnumerable<ExchangeOption>>(exchangeOptionDto);
                var transportOption = _mapper.Map<TransportOption>(transportOptionDto);
                await _mediatorForOptionsRep.AddDeviceOptionAsync(deviceOption, exchangeOption, transportOption);
                return CreatedAtAction("Get", new { deviceName = deviceOptionDto.Name }, data); //возвращает в ответе данные запроса. в Header пишет значение Location→ http://localhost:44138/api/DevicesOption/{deviceName}
            }
            catch (OptionHandlerException ex)
            {
                _logger.Error(ex, "Ошибка в DevicesOptionController/Post");
                ModelState.AddModelError("PostException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Критическая Ошибка в DevicesOptionController/Post");
                throw;
            }
        }



        // DELETE api/values/5
        [HttpDelete("{deviceName}")]
        public async Task<IActionResult> Delete([FromRoute]string deviceName)
        {
            var deviceOption = await _mediatorForOptionsRep.GetDeviceOptionByNameAsync(deviceName);
            if (deviceOption == null)
                return NotFound(deviceName);

            try
            {
                var deletedOption = await _mediatorForOptionsRep.RemoveDeviceOptionAsync(deviceOption);
                return Ok(deletedOption);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка в DevicesOptionController/Delete");
                throw;
            }
        }



        // POST api/devicesoption/BuildDevice/deviceName
        [HttpPost("BuildDevice/{deviceName}")]
        public async Task<IActionResult> BuildDevice([FromRoute] string deviceName)
        {
            try
            {
                var newDevice =  await _buildDeviceService.BuildDevice(deviceName);
                if (newDevice == null)
                {
                    return NotFound(deviceName);
                }
                return Ok(newDevice);
            }
            catch (StorageHandlerException ex)
            {
                _logger.Error(ex, "Ошибка в DevicesOptionController/BuildDevice");
                ModelState.AddModelError("BuildAndAddDeviceException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Критическая Ошибка в DevicesOptionController/BuildDevice");
                throw;
            }
        }

        #endregion
    }
}