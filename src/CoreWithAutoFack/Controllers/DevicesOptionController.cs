using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BL.Services.Mediators;
using BL.Services.Mediators.Exceptions;
using DAL.Abstract.Entities.Options;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Transport;
using Microsoft.AspNetCore.Mvc;
using WebServer.DTO.JSON.OptionsDto;
using WebServer.DTO.JSON.OptionsDto.DeviceOption;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption;
using WebServer.DTO.JSON.OptionsDto.TransportOption;

namespace WebServer.Controllers
{
    /// <summary>
    /// REST api доступа к опциям системы (Devices, Exchanges, Transports)
    /// </summary>
 
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DevicesOptionController : Controller
    {
        #region fields

        private readonly MediatorForOptions _mediatorForOptionsRep;
        private readonly MediatorForStorages _mediatorForStorages;
        private readonly IMapper _mapper;

        #endregion




        #region ctor

        public DevicesOptionController(MediatorForOptions mediatorForOptionsRep, MediatorForStorages mediatorForStorages, IMapper mapper)
        {
            _mediatorForOptionsRep = mediatorForOptionsRep;
            _mediatorForStorages = mediatorForStorages;
            _mapper = mapper;
        }


        //public DevicesOptionController(MediatorForOptions mediatorForOptionsRep, IMapper mapper)
        //{
        //    _mediatorForOptionsRep = mediatorForOptionsRep;
        //    _mapper = mapper;
        //}

        #endregion




        #region ApiMethode

        // GET api/devicesoption
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var deviceOptions = await _mediatorForOptionsRep.GetDeviceOptionsAsync();
                var exchangeOptions= await _mediatorForOptionsRep.GetExchangeOptionsAsync();
                var transportOption = await _mediatorForOptionsRep.GetTransportOptionsAsync();

                var deviceOptionsDto= _mapper.Map<List<DeviceOptionDto>>(deviceOptions);
                var exchangeOptionsDto= _mapper.Map<List<ExchangeOptionDto>>(exchangeOptions);
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
                Console.WriteLine(ex);
                //LOG
                throw;
            }    
        }



        // GET api/devicesoption/deviceName
        [HttpGet("{deviceName}", Name= "Get")]
        public async Task<IActionResult> Get([FromRoute]string deviceName)
        {
            try
            {
                var deviceOption= await _mediatorForOptionsRep.GetDeviceOptionByNameAsync(deviceName);
                if (deviceOption == null)
                    return NotFound(deviceName);

                var exchangesOptions= deviceOption.ExchangeKeys.Select(exchangeKey=> _mediatorForOptionsRep.GetExchangeByKeyAsync(exchangeKey).GetAwaiter().GetResult()).ToList();
                var transportOption= await _mediatorForOptionsRep.GetTransportByKeysAsync(exchangesOptions.Select(option=> option.KeyTransport).Distinct());
                var deviceOptionDto= _mapper.Map<DeviceOptionDto>(deviceOption);
                var exchangeOptionsDto= _mapper.Map<List<ExchangeOptionDto>>(exchangesOptions);
                var transportOptionDto= _mapper.Map<TransportOptionsDto>(transportOption);

                var agregatorOptionDto= new OptionAgregatorDto
                {
                    DeviceOptions = new List<DeviceOptionDto>{deviceOptionDto},
                    ExchangeOptions = exchangeOptionsDto,
                    TransportOptions = transportOptionDto
                };

                await Task.CompletedTask; //Debug
                return new JsonResult(agregatorOptionDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //LOG
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
                return CreatedAtAction("Get", new {deviceName= deviceOptionDto.Name}, data); //возвращает в ответе данные запроса. в Header пишет значение Location→ http://localhost:44138/api/DevicesOption/{deviceName}
            }
            catch (OptionHandlerException ex)
            {
                Console.WriteLine(ex);
                //LOG
                ModelState.AddModelError("PostException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //LOG
                throw;
            }        
        }



        // DELETE api/values/5
        [HttpDelete("{deviceName}")]
        public async Task<IActionResult> Delete([FromRoute]string deviceName)
        {
            var deviceOption= await _mediatorForOptionsRep.GetDeviceOptionByNameAsync(deviceName);
            if (deviceOption == null)
                return NotFound(deviceName);

            try
            {
                var deletedOption= await _mediatorForOptionsRep.RemoveDeviceOptionAsync(deviceOption);
                return Ok(deletedOption);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //LOG
                throw;
            }  
        }


        // POST api/devicesoption/BuildDevice/deviceName
        [Route("~/BuildDevice/{deviceName}")]
        [HttpPost("{deviceName}", Name = "BuildDevice")]
        public async Task<IActionResult> BuildDevice([FromRoute] string deviceName)             //TODO: доделать Route "api/devicesoption/BuildDevice/deviceName"
        {
            try
            {
                if (!await _mediatorForOptionsRep.IsExistDeviceAsync(deviceName))
                {
                    return NotFound(deviceName);
                }

               var optionAgregator= await _mediatorForOptionsRep.GetOptionAgregatorForDeviceAsync(deviceName);
                _mediatorForStorages.BuildAndAddDevice(optionAgregator);
                return Ok();
            }
            catch (StorageHandlerException ex)
            {
                Console.WriteLine(ex);
                //LOG
                ModelState.AddModelError("BuildAndAddDeviceException", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
    }
}