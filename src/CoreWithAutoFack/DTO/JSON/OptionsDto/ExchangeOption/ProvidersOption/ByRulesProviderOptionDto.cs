using System.Collections.Generic;

namespace WebServer.DTO.JSON.OptionsDto.ExchangeOption.ProvidersOption
{
    public class ByRulesProviderOptionDto
    {
        public List<RuleDto> RulesDto { get; set; }
    }



    public class RuleDto
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public RequestDto Request { get; set; }
        public ResponseDto Response { get; set; }
    }


    public class RequestDto
    {
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }


    public class ResponseDto
    {
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }

}