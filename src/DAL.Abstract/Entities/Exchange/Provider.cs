namespace DAL.Abstract.Entities.Exchange
{
    /// <summary>
    /// Конкретный провайдер обмена, захардкожженный (уазать имя)
    /// </summary>
    public class Provider : EntityBase
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int TimeRespone { get; set; }
    }
}