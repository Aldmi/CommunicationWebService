using Exchange.Base.Model;

namespace Exchange.Base.RxModel
{
    public class LastSendDataChangeRxModel<T>
    {
        public InDataWrapper<T> LastSendData { get; set;} 
        public string KeyExchange { get; set; }
    }
}