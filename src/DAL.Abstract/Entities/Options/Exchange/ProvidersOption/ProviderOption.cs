namespace DAL.Abstract.Entities.Options.Exchange.ProvidersOption
{
    public class ProviderOption
    {  
        public string Name { get; set; }
        public ByRulesProviderOption ByRulesProviderOption { get; set; }
        public ManualProviderOption ManualProviderOption { get; set; }
    }
}