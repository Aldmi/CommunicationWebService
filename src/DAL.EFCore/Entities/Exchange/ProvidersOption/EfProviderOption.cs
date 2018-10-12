namespace DAL.EFCore.Entities.Exchange.ProvidersOption
{
    public class EfProviderOption
    {  
        public string Name { get; set; }
        public EfByRulesProviderOption ByRulesProviderOption { get; set; }
        public EfManualProviderOption ManualProviderOption { get; set; }
    }
}