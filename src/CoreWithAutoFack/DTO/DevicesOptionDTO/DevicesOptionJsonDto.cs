using System.Collections.Generic;
using DAL.Abstract.Entities.Device;
using DAL.Abstract.Entities.Exchange;
using DAL.Abstract.Entities.Transport;


namespace WebServer.DTO.DevicesOptionDTO
{
    public class DevicesOptionJsonDto
    {
        public List<DeviceOption> DeviceOptions { get; set; }   
        public List<ExchangeOption> ExchangeOptions { get; set; }   
        public TransportOptionsDto TransportOptionsDto { get; set; }   
    }


    /// <summary>
    /// Коллекции опций для транспортов.
    /// </summary>
    public class TransportOptionsDto
    {
        public List<SerialOption> SerialOptions { get; set; }  
        public List<TcpIpOption> TcpIpOptions { get; set; }  
        public List<HttpOption> HttpOptions { get; set; }  
    }

}