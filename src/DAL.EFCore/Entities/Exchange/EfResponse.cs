using System.ComponentModel.DataAnnotations;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfResponse
    {
        [Key]
        public int Id { get; set; }
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }

        [Required]
        public string Body { get; set; }
    }
}