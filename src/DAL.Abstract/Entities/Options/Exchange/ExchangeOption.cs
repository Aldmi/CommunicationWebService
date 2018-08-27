using Shared.Types;

namespace DAL.Abstract.Entities.Options.Exchange
{
    public class ExchangeOption : EntityBase
    {
        public string Key { get; set; }
        public KeyTransport KeyTransport { get; set; }
        public ExchangeRule ExchangeRule { get; set; }
        public Provider Provider { get; set; }

        /// <summary>
        /// Добавление функции циклического обмена на бекгроунд
        /// Флаг учитывается, только при старте сервис.
        /// </summary>
        public bool AutoStartCycleFunc { get; set; }


        //public string MetaDataJsonForDb
        //{
        //    get
        //    {
        //        return MetaData == null || !MetaData.Any()
        //            ? null
        //            : JsonConvert.SerializeObject(MetaData);
        //    }

        //    set
        //    {
        //        if (string.IsNullOrWhiteSpace(value))
        //            MetaData.Clear();
        //        else
        //            MetaData = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
        //    }
        //}

    }
}