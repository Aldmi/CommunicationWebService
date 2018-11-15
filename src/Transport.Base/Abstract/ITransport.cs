using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Shared.Enums;
using Shared.Types;
using Transport.Base.DataProvidert;
using Transport.Base.RxModel;

namespace Transport.Base.Abstract
{
    public interface ITransport : ISupportKeyTransport, IDisposable
    {
        bool IsOpen { get; }                                                                   // ФЛАГ ОТКРЫТИЯ ПОРТА ТРАНСОРТА
        string StatusString { get; }                                                           // СОСТОЯНИЕ ТРАНСПОРТА
        StatusDataExchange StatusDataExchange { get; }                                         // СТАТУС ПОСЛЕДНЕГО ОБМЕНА
        bool IsCycleReopened { get; }                                                          // ФЛАГ НАХОЖДЕНИЯ ПОРТА В ЦИКЛЕ ПЕРЕОТКРЫТИЯ  

        ISubject<IsOpenChangeRxModel> IsOpenChangeRx { get; }                                  // СОБЫТИЕ ОТКРЫТИЯ/ЗАКРЫТИЯ ЛИНИИ КОММУНИКАЦИИ УСТРОЙСТВА
        ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeRx { get; }          // СОБЫТИЕ СМЕНЫ СОСТОЯНИЯ СТАТУСА ОБМЕНА 
        ISubject<StatusStringChangeRxModel> StatusStringChangeRx { get; }                      // СОБЫТИЕ СМЕНЫ СТРОКИ СТАТУСА ПОРТА

        Task<bool> CycleReOpened();                                                            // ПОПЫТКИ ЦИКЛИЧЕСКОГО ПЕРЕОТКРЫТИЯ ПОРТА (С УНИЧТОЖЕНИЕМ ТЕКУЩЕГО ЭКЗЕМПЛЯРА ПОРТА)
        void CycleReOpenedCancelation();                                                       // ОТМЕНА ЦИКЛИЧЕСКОГО ПЕРЕОТКРЫТИЯ
        Task<bool> ReOpen();                                                                         // ПЕРЕОТКРЫТЬ (БЕЗ УНИЧТОЖЕНИЕЯ ТЕКУЩЕГО ЭКЗЕМПЛЯРА ПОРТА)
        Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, ITransportDataProvider dataProvider, CancellationToken ct);   // ЗАПРОС/ОТВЕТ
    }
}