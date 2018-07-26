using System.Collections.Generic;

namespace WebServer.DTO.JSON.OptionsDto.TransportOption
{
    /// <summary>
    /// Коллекции опций для транспортов.
    /// </summary>
    public class TransportOptionsDto
    {
        public IEnumerable<SerialOptionDto> SerialOptions { get; set; }  
        public IEnumerable<TcpIpOptionDto> TcpIpOptions { get; set; }  
        public IEnumerable<HttpOptionDto> HttpOptions { get; set; }  
    }
}