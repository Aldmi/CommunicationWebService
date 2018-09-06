
using System.ComponentModel.DataAnnotations;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption.ProvidersOption;

namespace WebServer.DTO.JSON.OptionsDto.ExchangeOption.Providers
{
    public class ProviderOptionDto
    {  
        [Required(ErrorMessage = "Name для Provider не может быть NULL")]
        public string Name { get; set; } 

        public ByRulesProviderOptionDto ByRulesProviderOptionDto { get; set; }
        public ManualProviderOptionDto ManualProviderOptionDto { get; set; }
    }
}