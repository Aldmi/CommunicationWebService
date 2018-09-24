using System.Collections.Generic;

namespace InputDataModel.Base
{
    public class InputData<TIn>
    {
        /// <summary>
        /// Устройство
        /// </summary>
        public string DeviceName { get; set; }   
        
        /// <summary>
        /// Обмен
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public DataAction DataAction { get; set; }

        /// <summary>
        /// Данные
        /// </summary>
        public List<TIn> Data { get; set; }

        /// <summary>
        /// Команда
        /// </summary>
        public Command4Device Command { get; set; }
    }

    public enum DataAction {OneTimeAction, CycleAction, CommandAction}
    public enum Command4Device { None, On, Off, Restart, Clear}
}