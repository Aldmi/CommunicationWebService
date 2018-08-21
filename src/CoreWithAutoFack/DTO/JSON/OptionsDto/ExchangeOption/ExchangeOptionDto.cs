using System.ComponentModel.DataAnnotations;

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

        public ExchangeRuleDto ExchangeRule { get; set; }

        public ProviderDto Provider { get; set; }
    }
}