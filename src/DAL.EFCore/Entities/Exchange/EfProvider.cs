using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DAL.EFCore.Entities.Exchange
{

    public class EfProvider : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Range(1, 1000)]
        public int ProviderId { get; set; }

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