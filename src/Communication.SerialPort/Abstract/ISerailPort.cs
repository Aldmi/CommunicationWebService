using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Transport;
using Shared.Enums;
using Shared.Types;
using Transport.Base.DataProviderAbstract;
using Transport.Base.RxModel;



namespace Transport.SerialPort.Abstract
{
    public interface ISerailPort : ISupportKeyTransport, IDisposable
    {
        SerialOption SerialOption { get; }                                                     //НАСТРОЙКИ ПОРТА

        bool IsOpen { get; }                                                                   // ФЛАГ ОТКРЫТИЯ ПОРТА
        string StatusString { get; }                                                           // СТАТУС ПОРТА
        StatusDataExchange StatusDataExchange { get; }                                         // СТАТУС ОБМЕНА ПО ПОРТУ
        bool IsCycleReopened { get; }                                                          // ФЛАГ НАХОЖДЕНИЯ ПОРТА В ЦИКЛЕ ПЕРЕОТКРЫТИЯ  

        ISubject<IsOpenChangeRxModel> IsOpenChangeRx { get; }                                  // СОБЫТИЕ ОТКРЫТИЯ/ЗАКРЫТИЯ ПОРТА
        ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeRx { get; }          // СОБЫТИЕ СМЕНЫ СОСТОЯНИЯ СТАТУСА ОБМЕНА 
        ISubject<StatusStringChangeRxModel> StatusStringChangeRx { get; }                      // СОБЫТИЕ СМЕНЫ СТРОКИ СТАТУСА ПОРТА

        Task<bool> CycleReOpened();                                                            // ПОПЫТКИ ЦИКЛИЧЕСКОГО ПЕРЕОТКРЫТИЯ ПОРТА (С УНИЧТОЖЕНИЕМ ТЕКУЩЕГО ЭКЗЕМПЛЯРА ПОРТА)
        void CycleReOpenedCancelation();                                                       // ОТМЕНА ЦИКЛИЧЕСКОГО ПЕРЕОТКРЫТИЯ
        Task ReOpen();                                                                         // ПЕРЕОТКРЫТЬ (БЕЗ УНИЧТОЖЕНИЕЯ ТЕКУЩЕГО ЭКЗЕМПЛЯРА ПОРТА)
        Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, IExchangeDataProviderBase dataProvider, CancellationToken ct);   // ЗАПРОС/ОТВЕТ
    }
}