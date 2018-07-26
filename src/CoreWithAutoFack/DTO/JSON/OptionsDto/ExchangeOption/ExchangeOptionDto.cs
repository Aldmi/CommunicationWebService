using System.ComponentModel.DataAnnotations;

namespace WebServer.DTO.JSON.OptionsDto.ExchangeOption
{
    public class ExchangeOptionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Для Exchange, KeyTransport не может быть NULL")]
        public KeyTransportDto KeyTransport { get; set; }

        public ExchangeRuleDto ExchangeRule { get; set; }

        public ProviderDto Provider { get; set; }
    }
}