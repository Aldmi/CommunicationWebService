using System.ComponentModel.DataAnnotations;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Format { get; set; }

        [Required]
        public EfRequest EfRequest { get; set; }

        [Required]
        public EfRequest Response { get; set; }
    }

}