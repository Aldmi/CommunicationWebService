namespace WebServer.DTO.JSON.OptionsDto.ExchangeOption
{
    public class ExchangeOptionDto
    {
        public int Id { get; set; }
        public KeyTransportDto KeyTransport { get; set; }
        public ExchangeRuleDto ExchangeRule { get; set; }
        public ProviderDto Provider { get; set; }
    }
}