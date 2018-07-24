using System.Collections.Generic;

namespace WebServer.DTO.JSON.OptionsDto.TransportOption
{
    /// <summary>
    /// Коллекции опций для транспортов.
    /// </summary>
    public class TransportOptionsDto
    {
        public List<SerialOptionDto> SerialOptions { get; set; }  
        public List<TcpIpOptionDto> TcpIpOptions { get; set; }  
        public List<HttpOptionDto> HttpOptions { get; set; }  
    }
}