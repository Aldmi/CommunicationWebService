using System.ComponentModel.DataAnnotations;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfExchangeRule
    {
        [Key]
        public int Id { get; set; }
        public EfTable EfTable { get; set; }
    }
}