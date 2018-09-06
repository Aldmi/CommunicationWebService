namespace DAL.Abstract.Entities.Options.Exchange.Providers
{
    public class ProviderOption : EntityBase
    {
        public string Name { get; set; } // 
        public ByRulesProviderOption ByRulesProviderOption { get; set; }
        public ManualProviderOption ManualProviderOption { get; set; }
    }
}