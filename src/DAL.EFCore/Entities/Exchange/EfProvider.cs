using System.ComponentModel.DataAnnotations;


namespace DAL.EFCore.Entities.Exchange
{
    /// <summary>
    /// Конкретный провайдер обмена, захардкоженный (уазать имя)
    /// </summary>
    public class EfProvider
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        public int TimeRespone { get; set; }

        #region FK
        public EfExchangeOption EfExchangeOption { get; set; }
        public int? EfExchangeOptionId { get; set; }
        #endregion
    }
}