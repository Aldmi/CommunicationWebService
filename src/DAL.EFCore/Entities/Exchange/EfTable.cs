using System.ComponentModel.DataAnnotations;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfTable : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int Size { get; set; }
        public int Position { get; set; }

        [Required]
        public EfRule EfRule { get; set; }
    }
}