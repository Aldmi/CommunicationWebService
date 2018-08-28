using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Shared.Types;

namespace DAL.EFCore.Entities.Transport
{
    public class EfHttpOption : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Address { get; set; }


        public bool AutoStart { get; set; }


        private string _headersMetaData;
        [NotMapped]
        public Dictionary<string, string> Headers
        {
            get => string.IsNullOrEmpty(_headersMetaData) ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(_headersMetaData);
            set => _headersMetaData = (value == null) ? null : JsonConvert.SerializeObject(value);
        }
    }
}