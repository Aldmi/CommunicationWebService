using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Shared.Enums;
using Transport.Base.DataProviderAbstract;
using Transport.SerialPort.Option;
using Transport.SerialPort.RxModel;


namespace Transport.SerialPort.Abstract
{
    public interface ISerailPort : IDisposable
    {
        SerialOption SerialOption { get; }                                                     //НАСТРОЙКИ ПОРТА

        bool IsOpen { get; }                                                                   // ФЛАГ ОТКРЫТИЯ ПОРТА
        string StatusString { get; }                                                           // СТАТУС ПОРТА
        StatusDataExchange StatusDataExchange { get; }                                         // СТАТУС ОБМЕНА ПО ПОРТУ
        bool IsCycleReconnectState { get; }                                                    // ФЛАГ НАХОЖДЕНИЯ ПОРТА В ЦИКЛЕ ПЕРЕОТКРЫТИЯ  

        ISubject<IsOpenChangeRxModel> IsOpenChangeRx { get; }                                  // СОБЫТИЕ ОТКРЫТИЯ/ЗАКРЫТИЯ ПОРТА
        ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeRx { get; }          // СОБЫТИЕ СМЕНЫ СОСТОЯНИЯ СТАТУСА ОБМЕНА 
        ISubject<StatusStringChangeRxModel> StatusStringChangeRx { get; }                      // СОБЫТИЕ СМЕНЫ СТРОКИ СТАТУСА ПОРТА

        Task<bool> CycleReConnect();                                                           // ПОПЫТКИ ЦИКЛИЧЕСКОГО ПЕРЕОТКРЫТИЯ ПОРТА (С УНИЧТОЖЕНИЕМ ТЕКУЩЕГО ЭКЗЕМПЛЯРА ПОРТА)
        void CycleReConnectCancelation();                                                      // ОТМЕНА ЦИКЛИЧЕСКОГО ПЕРЕОТКРЫТИЯ
        Task ReOpen();                                                                         // ПЕРЕОТКРЫТЬ (БЕЗ УНИЧТОЖЕНИЕЯ ТЕКУЩЕГО ЭКЗЕМПЛЯРА ПОРТА)
        Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, IExchangeDataProviderBase dataProvider, CancellationToken ct);   // ЗАПРОС/ОТВЕТ
    }
}