using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Base.Model;
using Transport.Base.DataProvidert;

namespace Exchange.Base.DataProviderAbstract
{
    public interface IExchangeDataProvider<TInput, TOutput> : ITransportDataProvider
    {
        InDataWrapper<TInput> InputData { get; set; }                //передача входных даных внешним кодом.
        TOutput OutputData { get; set; }                             //возврат выходных данных во внешний код.
        bool IsOutDataValid { get; }                                 // флаг валидности выходных данных (OutputData)

        Subject<TOutput> OutputDataChangeRx { get; }                 //Событие получения выходных данных
        string ProviderName { get; set; }

        Subject<ITransportDataProvider> RaiseSendDataRx { get; }     //Событие отправки входных данных, в процессе обработки их конвеером.
        Task StartExchangePipline(InDataWrapper<TInput> inData);     //Запустить конвеер обмена. После окончания подготовки порции данных конвеером, срабатывает RaiseSendDataRx.
        int TimeRespone { get; }                                     //Время на ответ
    }
}