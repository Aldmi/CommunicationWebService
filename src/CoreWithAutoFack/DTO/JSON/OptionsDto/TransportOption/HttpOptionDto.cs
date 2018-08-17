using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebServer.DTO.JSON.OptionsDto.TransportOption
{
    public class HttpOptionDto
    {
        public int Id { get; set; }

        public bool AutoStart { get; set; }

        [Required(ErrorMessage = "Имя Http  не может быть NULL")]
        public string Name { get; set; }

        
        [RegularExpression(@"/^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/")]
        public string Address { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}