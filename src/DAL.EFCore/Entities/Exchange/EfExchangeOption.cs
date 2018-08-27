using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.Enums;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfExchangeOption : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 1000)]
        public int ExchangeId { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public EfKeyTransport KeyTransport { get; set; }

        //public EfExchangeRule EfExchangeRule { get; set; } //Или Rule или Provider

        public EfProvider Provider { get; set; }

        public bool AutoStartCycleFunc { get; set; }
    }


    public class EfKeyTransport
    {
        [Key]
        public int Id { get; set; } 

        #region FK
        public int EfExchangeOptionId { get; set; }
        public EfExchangeOption EfExchangeOption { get; set; }
        #endregion


        public string Key { get; set; }
        public TransportType TransportType { get; set; }
    }
}