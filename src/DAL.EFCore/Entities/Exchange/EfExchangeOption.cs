﻿using System.Collections.Generic;
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
        public int Id { get; set; }

        [Range(1, 1000)]
        public int ExchangeId { get; set; }

        [Required]
        public string Key { get; set; }

        //public EfExchangeRule EfExchangeRule { get; set; } //Или Rule или Provider

        public EfProvider Provider { get; set; }

        public bool AutoStartCycleFunc { get; set; }


        private string _keyTransportMetaData;
        [NotMapped]
        public KeyTransport KeyTransport
        {
            get => string.IsNullOrEmpty(_keyTransportMetaData) ? null : JsonConvert.DeserializeObject<KeyTransport>(_keyTransportMetaData);
            set => _keyTransportMetaData = (value == null) ? null : JsonConvert.SerializeObject(value);
        }
    }
}