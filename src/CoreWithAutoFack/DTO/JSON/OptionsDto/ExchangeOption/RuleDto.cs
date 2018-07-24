namespace WebServer.DTO.JSON.OptionsDto.ExchangeOption
{
    public class RuleDto
    {
        public int Id { get; set; }
        public string Format { get; set; }
        public RequestDto Request { get; set; }
        public RequestDto Response { get; set; }
    }
}