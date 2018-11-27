using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Exchange.Base.Model;
using Transport.Base.DataProvidert;

namespace Exchange.Base.DataProviderAbstract
{
    public interface IExchangeDataProvider<TInput, TOutput> : ITransportDataProvider
    {
        InDataWrapper<TInput> InputData { get; set; }                //передача входных даных внешним кодом.
        TOutput OutputData { get; set; }                             //возврат выходных данных во внешний код.
        bool IsOutDataValid { get; }                                 //флаг валидности выходных данных (OutputData)
         
        string ProviderName { get;  }                                 //Название провайдера
        StringBuilder StatusString { get; }                           //Статус провайдера.
        int TimeRespone { get; }                                      //Время на ответ

        Task<int> StartExchangePipeline(InDataWrapper<TInput> inData);                     //Запустить конвеер обмена. После окончания подготовки порции данных конвеером, срабатывает RaiseSendDataRx.
        Subject<IExchangeDataProvider<TInput, TOutput>> RaiseSendDataRx { get; }     //Событие отправки данных, в процессе обработки их конвеером.
    }
}