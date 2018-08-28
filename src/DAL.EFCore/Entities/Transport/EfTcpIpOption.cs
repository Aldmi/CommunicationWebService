using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.EFCore.Entities.Transport
{
    public class EfTcpIpOption : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
    
        public bool AutoStart { get; set; }
    }
}