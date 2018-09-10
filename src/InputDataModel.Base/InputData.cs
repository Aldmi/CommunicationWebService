using System.Collections.Generic;

namespace InputDataModel.Base
{
    public class InputData<TIn>
    {
        //TODO: Добавить данные для идентификации устройства -> обмена -> действия 
        public string DeviceName { get; set; }
        public string ExchangeName { get; set; }
        public List<TIn> Data { get; set; }
    }
}