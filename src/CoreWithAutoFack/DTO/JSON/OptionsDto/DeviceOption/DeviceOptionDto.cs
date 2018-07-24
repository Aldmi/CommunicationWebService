using System.Collections.Generic;

namespace WebServer.DTO.JSON.OptionsDto.DeviceOption
{
    public class DeviceOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<KeyTransportDto> KeyTransports { get; set; }
    }
}