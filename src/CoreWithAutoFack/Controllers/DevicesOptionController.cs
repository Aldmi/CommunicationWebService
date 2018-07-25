using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;
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

        private readonly ISerialPortOptionRepository _spOptionRep;
        private readonly ITcpIpOptionRepository _tcpIpOptionRep;
        private readonly IHttpOptionRepository _httpOptionRep;
        private readonly IExchangeOptionRepository _exchangeOptionRep;
        private readonly IDeviceOptionRepository _deviceOptionRep;
        private readonly IMapper _mapper;

        #endregion




        #region ctor

        public DevicesOptionController(ISerialPortOptionRepository spOptionRep,
            ITcpIpOptionRepository tcpIpOptionRep,
            IHttpOptionRepository httpOptionRep,
            IExchangeOptionRepository exchangeOptionRep,
            IDeviceOptionRepository deviceOptionRep,
            IMapper mapper)
        {
            _spOptionRep = spOptionRep;
            _tcpIpOptionRep = tcpIpOptionRep;
            _httpOptionRep = httpOptionRep;
            _exchangeOptionRep = exchangeOptionRep;
            _deviceOptionRep = deviceOptionRep;
            _mapper = mapper;
        }

        #endregion





        // GET api/devicesoption
        [HttpGet]
        public async Task<AgregatorOptionDto> Get()
        {
            try
            {
                var deviceOptions= _deviceOptionRep.List().ToList();
                var exchangeOptions= _exchangeOptionRep.List().ToList();
                var serialOptions= _spOptionRep.List().ToList();
                var tcpIpOptions= _tcpIpOptionRep.List().ToList();
                var httpOptions= _httpOptionRep.List().ToList();
      
                var deviceOptionsDto= _mapper.Map<List<DeviceOptionDto>>(deviceOptions);
                var exchangeOptionsDto= _mapper.Map<List<ExchangeOptionDto>>(exchangeOptions);
                var serialOptionsDto= _mapper.Map<List<SerialOptionDto>>(serialOptions);
                var tcpIpOptionsDto= _mapper.Map<List<TcpIpOptionDto>>(tcpIpOptions);
                var httpOptionsDto= _mapper.Map<List<HttpOptionDto>>(httpOptions);
       
                var deviceOptionDto = new AgregatorOptionDto
                {
                    DeviceOptions = deviceOptionsDto,
                    ExchangeOptions = exchangeOptionsDto,
                    TransportOptionsDto = new TransportOptionsDto
                    {
                        SerialOptions = serialOptionsDto,
                        TcpIpOptions = tcpIpOptionsDto,
                        HttpOptions = httpOptionsDto
                    }
                };
                await Task.Delay(100);//DEBUG
                return deviceOptionDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //LOG
                return null;
            }
        }



        // POST api/devicesoption
        [HttpPost]
        public IActionResult Post([FromBody]AgregatorOptionDto data)
        {         
            if (data==null)
            {
                ModelState.AddModelError("AgregatorOptionDto", "POST body is null");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deviceOptionDto= data.DeviceOptions.FirstOrDefault();     
            var deviceOption= _mapper.Map<DeviceOption>(deviceOptionDto);


           var spOptionDto= data.TransportOptionsDto.SerialOptions.FirstOrDefault();
           var spOption = _mapper.Map<SerialOption>(spOptionDto);

            //1. Добавить транспорт, его может не быть 
            //2. Добавить обмен, его может не быть
            //3. Добавить Device.

            return Ok();
        }



        //DEBUG---------------------------------------------------------------------------

        //[HttpGet]
        //[Route("TestDto")]
        //public async Task<TestDto> TestDtoGet()
        //{
        //   var testDto= new TestDto()
        //   {
        //       IsActive = true,
        //       Name = "sddssfsgfd",
        //       InnerType = new InnerType{ Id = 128}
        //       //KeyTransport = new KeyTransport ("COM1", TransportType.SerialPort)
        //   };
        //    await Task.Delay(100);

        //    return testDto;
        //}

        //[HttpPost]
        //[Route("TestDto")]
        //public void Post([FromBody]TestDto value)
        //{

        //}

        //DEBUG---------------------------------------------------------------------------


    }
}