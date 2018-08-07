using Shared.Types;

namespace DAL.Abstract.Entities.Options.Exchange
{
    public class ExchangeOption : EntityBase
    {
        public string Key { get; set; }
        public KeyTransport KeyTransport { get; set; }
        public ExchangeRule ExchangeRule { get; set; }
        public Provider Provider { get; set; }

        /// <summary>ъ
        /// Запуск Бегкроунда обмена.
        /// Флаг учитывается, только при старте сервис.
        /// </summary>
        public bool AutoStartBackground { get; set; }

        /// <summary>
        /// Добавление функции циклического обмена на бекгроунд
        /// Флаг учитывается, только при старте сервис.
        /// </summary>
        public bool AutoStartCycleFunc { get; set; }
    }
}