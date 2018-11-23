using System.ComponentModel.DataAnnotations;

namespace WebServer.DTO.JSON.OptionsDto.TransportOption
{
    public class TcpIpOptionDto
    {
        public int Id { get; set; }

        public bool AutoStart { get; set; }

        [Required(ErrorMessage = "Имя TCP/IP  не может быть NULL")]
        public string Name { get; set; }

        [RegularExpression(@"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.
                             (25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.
                             (25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.
                             (25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)
                             (\:([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))?$")]  //192.168.1.1:5000
        public string IpAddress { get; set; }             //Ip

        public int IpPort { get; set; }                  //порт      


    }
}