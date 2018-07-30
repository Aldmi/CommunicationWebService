using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BL.Services.Mediators;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;
using DAL.Abstract.Entities.Exchange;
using DAL.Abstract.Entities.Transport;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Types;
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

        private readonly MediatorForOptionsRepository _mediatorForOptionsRep;
        private readonly IMapper _mapper;

        #endregion




        #region ctor

        public DevicesOptionController(MediatorForOptionsRepository mediatorForOptionsRep, IMapper mapper)
        {
            _mediatorForOptionsRep = mediatorForOptionsRep;
            _mapper = mapper;
        }

        #endregion




        #region ApiMethode

        // GET api/devicesoption
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var deviceOptions = _mediatorForOptionsRep.GetDeviceOptions().ToList();
                var exchangeOptions= _mediatorForOptionsRep.GetExchangeOptions().ToList();
                var transportOption = _mediatorForOptionsRep.GetTransportOptions();

                var deviceOptionsDto= _mapper.Map<List<DeviceOptionDto>>(deviceOptions);
                var exchangeOptionsDto= _mapper.Map<List<ExchangeOptionDto>>(exchangeOptions);
                var transportOptionDto = _mapper.Map<TransportOptionsDto>(transportOption);

                var agregatorOptionDto = new AgregatorOptionDto
                {
                    DeviceOptions = deviceOptionsDto,
                    ExchangeOptions = exchangeOptionsDto,
                    TransportOptions = transportOptionDto
                };



                await Task.Delay(0);//DEBUG
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
        [HttpGet("{deviceName}")]
        public async Task<IActionResult> Get([FromRoute]string deviceName)
        {
            try
            {
                var deviceOption= _mediatorForOptionsRep.GetDeviceOptionByName(deviceName);
                if (deviceOption == null)
                    return NotFound(deviceName);

                var exchangesOptions= deviceOption.ExchangeKeys.Select(exchangeKey=> _mediatorForOptionsRep.GetExchangeByKey(exchangeKey)).ToList();
                var transportOption= _mediatorForOptionsRep.GetTransportByKeys(exchangesOptions.Select(option=> option.KeyTransport));

                var deviceOptionDto = _mapper.Map<DeviceOptionDto>(deviceOption);
                var exchangeOptionsDto = _mapper.Map<List<ExchangeOptionDto>>(exchangesOptions);
                var transportOptionDto = _mapper.Map<TransportOptionsDto>(transportOption);

                var agregatorOptionDto = new AgregatorOptionDto
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
        public IActionResult Post([FromBody]AgregatorOptionDto data)
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
                _mediatorForOptionsRep.AddDeviceOption(deviceOption, exchangeOption, transportOption);
                return Ok(data);
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

            await Task.Delay(0);//DEBUG
            return Ok();
        }

        #endregion
    }
}