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





        // GET api/devicesoption
        [HttpGet]
        public async Task<AgregatorOptionDto> Get()
        {
            try
            {
                var deviceOptions = _mediatorForOptionsRep.GetDeviceOptions().ToList();
                var exchangeOptions= _mediatorForOptionsRep.GetExchangeOptions().ToList();
                var transportOption = _mediatorForOptionsRep.GetTransportOptions();

                var deviceOptionsDto= _mapper.Map<List<DeviceOptionDto>>(deviceOptions);
                var exchangeOptionsDto= _mapper.Map<List<ExchangeOptionDto>>(exchangeOptions);
                var transportOptionDto = _mapper.Map<TransportOptionsDto>(transportOption);

                var deviceOptionDto = new AgregatorOptionDto
                {
                    DeviceOptions = deviceOptionsDto,
                    ExchangeOptions = exchangeOptionsDto,
                    TransportOptions = transportOptionDto
                };

                //throw new Exception("fdfdf");

                await Task.Delay(100);//DEBUG
                return deviceOptionDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //LOG
                return null;     //TODO: Как пересылать ошибки на GET запрос
            }
        }



        // GET api/devicesoption/deviceName
        [HttpGet("{deviceName}")]
        public async Task<AgregatorOptionDto> Get(string deviceName)
        {
            try
            {
                //TODO: возвращает Device option со всеми обменами и транспортом

                var deviceOptions = _mediatorForOptionsRep.GetDeviceOptions().ToList();
                var exchangeOptions = _mediatorForOptionsRep.GetExchangeOptions().ToList();
                var transportOption = _mediatorForOptionsRep.GetTransportOptions();

                var deviceOptionsDto = _mapper.Map<List<DeviceOptionDto>>(deviceOptions);
                var exchangeOptionsDto = _mapper.Map<List<ExchangeOptionDto>>(exchangeOptions);
                var transportOptionDto = _mapper.Map<TransportOptionsDto>(transportOption);

                var deviceOptionDto = new AgregatorOptionDto
                {
                    DeviceOptions = deviceOptionsDto,
                    ExchangeOptions = exchangeOptionsDto,
                    TransportOptions = transportOptionDto
                };

                //throw new Exception("fdfdf");

                await Task.Delay(100);//DEBUG
                return deviceOptionDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //LOG
                return null;     //TODO: Как пересылать ошибки на GET запрос
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //LOG
                ModelState.AddModelError("AgregatorOptionDto", e.Message);
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}