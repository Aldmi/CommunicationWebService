using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Exchange.Base.Model;
using Shared.Types;
using Transport.Base.RxModel;
using Worker.Background.Abstarct;

namespace Exchange.Base
{
    /// <summary>
    /// УНИВЕРСАЛЬНЫЙ ОБМЕН ДАННЫМИ СО ВСЕМИ УСТРОЙСТВАМИ.
    /// </summary>
    public interface IExchange : ISupportExchangeKey
    {
        #region StateExchange
        bool IsOpen { get; }                                                      //Соединение открыто
        bool IsConnect { get; }                                                   //Устройсвто на связи по открытому соединению (определяется по правильным ответам от ус-ва)
        UniversalInputType LastSendData { get; }                                  //Последние отосланные данные
        IEnumerable<string> GetRuleNames { get; }                                 //Отдать название установленных правил обмена
        #endregion


        #region StartStop
        Task CycleReOpened();
        void CycleReOpenedCancelation();
        void StartCycleExchange();                                                             //Запустить цикл. обмен (ДОБАВИТЬ функцию цикл обмена на бекграунд)
        void StopCycleExchange();                                                              //Остановить цикл. обмен (УДАЛИТЬ функцию цикл обмена из бекграунд)
        #endregion


        #region SendData
        void SendCommand(string commandName, UniversalInputType data4Command = null);           //однократно выполняемая команда
        void SendOneTimeData(UniversalInputType inData);                                        //однократно отсылаемые данные (если указанны правила, то только для этих правил)
        void SendCycleTimeData(UniversalInputType inData);                                      //циклически отсылаемые данные
        #endregion


        #region ExchangeRx
        ISubject<IExchange> IsDataExchangeSuccessChange { get; }
        ISubject<IExchange> IsConnectChange { get; }
        ISubject<IExchange> LastSendDataChange { get; }
        #endregion


        #region TransportRx
        ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx { get; }                                  //ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. ОТКРЫТИЯ/ЗАКРЫТИЯ ПОРТА
        ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx { get; }          // ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. СМЕНЫ СОСТОЯНИЯ СТАТУСА ОБМЕНА 
        ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx { get; }                      // ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. СМЕНЫ СТРОКИ СТАТУСА ПОРТА
        #endregion         
    }
}