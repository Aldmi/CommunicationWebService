using System.Collections.Generic;

namespace Exchange.Base.Model.InputData
{
    /// <summary>
    /// Входные данные для устройств.
    /// </summary>
    public class InputDataProtocol
    {
        
    }


    public class DeviceInputData
    {
        public string Name { get; set; }
        public List<ExchangeInputData> ExchangeInputDatas { get; set; } 
    }


    public class ExchangeInputData
    {
        public string ExchangeKey { get; set; }
    }

    public class Data
    {

    }

}