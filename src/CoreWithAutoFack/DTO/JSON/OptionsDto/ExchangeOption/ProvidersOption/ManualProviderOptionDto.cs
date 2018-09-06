namespace WebServer.DTO.JSON.OptionsDto.ExchangeOption.ProvidersOption
{
    /// <summary>
    /// Конкретный провайдер обмена, захардкоженный (указать имя)
    /// </summary>
    public class ManualProviderOptionDto
    {
        public string Address { get; set; }
        public int TimeRespone { get; set; }
    }
}