using System.ComponentModel.DataAnnotations;
using DAL.Abstract.Entities.Options.Transport;

namespace WebServer.DTO.JSON.OptionsDto.TransportOption
{
    public class SerialOptionDto
    {
        public int Id { get; set; }

        public bool AutoStart { get; set; }

        [Required(ErrorMessage = "Имя последовательного порта не может быть NULL")]
        public string Port { get; set; }

        [RegularExpression(@"9600|19200|38400|57600|115200")]
        public int BaudRate { get; set; }

        [Range(5,8)]
        public int DataBits { get; set; }

        [EnumDataType(typeof(StopBits))]      
        public StopBits StopBits { get; set; }

        [EnumDataType(typeof(Parity))]    
        public Parity Parity { get; set; }


        public bool DtrEnable { get; set; }

        public bool RtsEnable { get; set; }
    }
}