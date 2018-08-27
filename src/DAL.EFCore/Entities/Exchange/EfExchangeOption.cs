using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.Enums;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfExchangeOption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public EfKeyTransport KeyTransport { get; set; }

        //public EfExchangeRule EfExchangeRule { get; set; } //Или Rule или Provider
        public EfProvider EfProvider { get; set; }

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