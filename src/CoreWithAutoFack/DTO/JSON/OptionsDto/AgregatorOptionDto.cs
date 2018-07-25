using System.Collections.Generic;
using Shared.Types;
using WebServer.DTO.JSON.OptionsDto.DeviceOption;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption;
using WebServer.DTO.JSON.OptionsDto.TransportOption;

namespace WebServer.DTO.JSON.OptionsDto
{
    public class AgregatorOptionDto
    {
        public List<DeviceOptionDto> DeviceOptions { get; set; }   
        public List<ExchangeOptionDto> ExchangeOptions { get; set; }   
        public TransportOptionsDto TransportOptionsDto { get; set; }   
    }




    //DEBUG-----------------

    public class TestDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public InnerType InnerType { get; set; }
        public KeyTransport KeyTransport { get; set; }
    }

    public class InnerType
    {
        public int Id { get; set; }
    }

    
    //DEBUG-----------------
}