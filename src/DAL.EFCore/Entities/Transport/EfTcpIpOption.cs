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
    
        [Required]
        [RegularExpression(@"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)")]
        public string IpAddress { get; set; }         
        
        [Range(0,65535)]
        public int IpPort { get; set; }                 

        public bool AutoStart { get; set; }
    }
}