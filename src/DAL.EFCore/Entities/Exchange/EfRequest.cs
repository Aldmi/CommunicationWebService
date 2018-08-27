using System.ComponentModel.DataAnnotations;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfRequest : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int MaxLenght { get; set; }

        [Required]
        public string Body { get; set; }
    }
}