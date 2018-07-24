using System.Collections.Generic;

namespace WebServer.DTO.JSON.OptionsDto.TransportOption
{
    public class HttpOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}