using System.Reactive.Subjects;
using Transport.Base.DataProvidert;

namespace DataProvider.Base.Abstract
{
    public interface IExchangeDataProvider<TInput, TOutput> : ITransportDataProvider
    {
        TInput InputData { get; set; }     //передача входных даных внешним кодом.
        TOutput OutputData { get; set; }   //возврат выходных данных во внешний код.
        bool IsOutDataValid { get; }       // флаг валидности выходных данных (OutputData)

        Subject<TOutput> OutputDataChangeRx { get; } //Событие получения выходных данных
        string ProviderName { get; set; }
    }
}