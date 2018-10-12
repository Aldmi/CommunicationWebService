using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.EFCore.Entities.Exchange.ProvidersOption;
using Newtonsoft.Json;
using Shared.Enums;

namespace DAL.EFCore.Entities.Exchange
{
    public class EfExchangeOption : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        public bool AutoStartCycleFunc { get; set; }

        public int CountBadTrying { get; set; }

        private string _keyTransportMetaData;
        [NotMapped]
        public EfKeyTransport KeyTransport
        {
            get => string.IsNullOrEmpty(_keyTransportMetaData) ? null : JsonConvert.DeserializeObject<EfKeyTransport>(_keyTransportMetaData);
            set => _keyTransportMetaData = (value == null) ? null : JsonConvert.SerializeObject(value);
        }

        private string _providerOptionMetaData;
        [NotMapped]
        public EfProviderOption Provider
        {
            get => string.IsNullOrEmpty(_providerOptionMetaData) ? null : JsonConvert.DeserializeObject<EfProviderOption>(_providerOptionMetaData);
            set => _providerOptionMetaData = (value == null) ? null : JsonConvert.SerializeObject(value);
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