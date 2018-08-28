using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Types;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfExchangeOption : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        //public EfExchangeRule EfExchangeRule { get; set; } //Или Rule или Provider

        public EfProvider Provider { get; set; }

        public bool AutoStartCycleFunc { get; set; }


        private string _keyTransportMetaData;
        [NotMapped]
        public EfKeyTransport KeyTransport
        {
            get => string.IsNullOrEmpty(_keyTransportMetaData) ? null : JsonConvert.DeserializeObject<EfKeyTransport>(_keyTransportMetaData);
            set => _keyTransportMetaData = (value == null) ? null : JsonConvert.SerializeObject(value);
        }
    }

    public class EfKeyTransport
    {
        #region prop

        public string Key { get; set; }
        public TransportType TransportType { get; set; }

        #endregion
    }

}