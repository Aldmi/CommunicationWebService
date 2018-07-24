using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Types;
using WebServer.DTO.JSON.OptionsDto;

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

        #endregion




        #region ctor

        public DevicesOptionController(ISerialPortOptionRepository spOptionRep,
            ITcpIpOptionRepository tcpIpOptionRep,
            IHttpOptionRepository httpOptionRep,
            IExchangeOptionRepository exchangeOptionRep,
            IDeviceOptionRepository deviceOptionRep)
        {
            _spOptionRep = spOptionRep;
            _tcpIpOptionRep = tcpIpOptionRep;
            _httpOptionRep = httpOptionRep;
            _exchangeOptionRep = exchangeOptionRep;
            _deviceOptionRep = deviceOptionRep;
        }

        #endregion





        // GET api/devicesoption
        [HttpGet]
        public async Task<DevicesOptionJsonDto> Get()
        {
            var deviceOptionDto = new DevicesOptionJsonDto
            {
                //DeviceOptions = _deviceOptionRep.List().ToList(),
                //ExchangeOptions = _exchangeOptionRep.List().ToList(),
                //TransportOptionsDto = new TransportOptionsDto
                //{
                //    SerialOptions = _spOptionRep.List().ToList(),
                //    TcpIpOptions = _tcpIpOptionRep.List().ToList(),
                //    HttpOptions = _httpOptionRep.List().ToList()
                //}
            };

            await Task.Delay(100);

            return deviceOptionDto;
        }



        // POST api/devicesoption
        [HttpPost]
        public void Post(string value)
        {

        }



        //DEBUG---------------------------------------------------------------------------

        [HttpGet]
        [Route("TestDto")]
        public async Task<TestDto> TestDtoGet()
        {
           var testDto= new TestDto()
           {
               IsActive = true,
               Name = "sddssfsgfd",
               InnerType = new InnerType{ Id = 128}
               //KeyTransport = new KeyTransport ("COM1", TransportType.SerialPort)
           };
            await Task.Delay(100);

            return testDto;
        }

        [HttpPost]
        [Route("TestDto")]
        public void Post([FromBody]TestDto value)
        {

        }

        //DEBUG---------------------------------------------------------------------------


    }
}