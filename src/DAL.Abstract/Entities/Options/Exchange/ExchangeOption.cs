using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using Shared.Types;

namespace DAL.Abstract.Entities.Options.Exchange
{
    public class ExchangeOption : EntityBase
    {
        public string Key { get; set; }
        public KeyTransport KeyTransport { get; set; }
        public ProviderOption Provider { get; set; }
        public int CountBadTrying { get; set; }

        /// <summary>
        /// Добавление функции циклического обмена на бекгроунд
        /// Флаг учитывается, только при старте сервис.
        /// </summary>
        public bool AutoStartCycleFunc { get; set; }
    }
}