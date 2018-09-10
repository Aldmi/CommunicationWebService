using System.Collections.Generic;
using WebServer.DTO.JSON.OptionsDto.DeviceOption;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption;
using WebServer.DTO.JSON.OptionsDto.TransportOption;


namespace WebServer.DTO.JSON.OptionsDto
{
    public class OptionAgregatorDto
    {
        public List<DeviceOptionDto> DeviceOptions { get; set; }   
        public List<ExchangeOptionDto> ExchangeOptions { get; set; }   
        public TransportOptionsDto TransportOptions { get; set; }   
    }
}