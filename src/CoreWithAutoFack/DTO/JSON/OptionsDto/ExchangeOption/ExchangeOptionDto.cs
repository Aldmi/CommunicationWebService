using System.ComponentModel.DataAnnotations;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption.Providers;

namespace WebServer.DTO.JSON.OptionsDto.ExchangeOption
{
    public class ExchangeOptionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Key для Exchange не может быть NULL")]
        public string Key { get; set; }

        public bool AutoStartCycleFunc { get; set; }

        [Required(ErrorMessage = "KeyTransport для Exchange не может быть NULL")]
        public KeyTransportDto KeyTransport { get; set; }

        [Required(ErrorMessage = "Provider для Exchange не может быть NULL")]
        public ProviderOptionDto Provider { get; set; }
    }
}