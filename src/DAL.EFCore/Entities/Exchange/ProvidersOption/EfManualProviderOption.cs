namespace DAL.EFCore.Entities.Exchange.ProvidersOption
{
    /// <summary>
    /// Конкретный провайдер обмена, захардкоженный (указать имя)
    /// </summary>
    public class EfManualProviderOption
    {
        public string Address { get; set; }
        public int TimeRespone { get; set; }
    }
}