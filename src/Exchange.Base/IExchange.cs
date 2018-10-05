using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Exchange.Base.Model;
using InputDataModel.Base;
using Shared.Types;
using Transport.Base.RxModel;

namespace Exchange.Base
{
    /// <summary>
    /// УНИВЕРСАЛЬНЫЙ ОБМЕН ДАННЫМИ СО ВСЕМИ УСТРОЙСТВАМИ.
    /// </summary>
    public interface IExchange<T> : ISupportKeyTransport, IDisposable
    {
        #region ByOption
        string KeyExchange { get; } 
        bool AutoStartCycleFunc { get; }
        #endregion


        #region StateExchange
        bool IsOpen { get; }                                                      //Соединение открыто
        bool IsConnect { get; }                                                   //Устройсвто на связи по открытому соединению (определяется по правильным ответам от ус-ва)
        bool IsStartedTransportBg { get; }                                        //Запущен бекграунд на транспорте
        InDataWrapper<T> LastSendData { get; }                                    //Последние отосланные данные 
        bool IsStartedCycleExchange { get; set; }                                 //Флаг цикл. обмена
        #endregion


        #region StartStop
        Task CycleReOpened();
        void CycleReOpenedCancelation();
        void StartCycleExchange();                                                             //Запустить цикл. обмен (ДОБАВИТЬ функцию цикл обмена на бекграунд)
        void StopCycleExchange();                                                              //Остановить цикл. обмен (УДАЛИТЬ функцию цикл обмена из бекграунд)
        #endregion


        #region SendData
        void SendCommand(Command4Device command);              //однократно выполняемая команда
        void SendOneTimeData(IEnumerable<T> inData);                           //однократно отсылаемые данные (если указанны правила, то только для этих правил)
        void SendCycleTimeData(IEnumerable<T> inData);                         //циклически отсылаемые данные
        #endregion


        #region ExchangeRx
        ISubject<IExchange<T>> IsDataExchangeSuccessChangeRx { get; }                         //TODO: сделать строго типизированные события NameRxModel
        ISubject<IExchange<T>> IsConnectChangeRx { get; }
        ISubject<IExchange<T>> LastSendDataChangeRx { get; }

        ISubject<OutResponseDataWrapper<T>> TransportResponseChangeRx { get; }
        #endregion


        #region TransportRx
        ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx { get; }                                  //ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. ОТКРЫТИЯ/ЗАКРЫТИЯ ПОРТА
        ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx { get; }          //ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. СМЕНЫ СОСТОЯНИЯ СТАТУСА ОБМЕНА 
        ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx { get; }                      //ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. СМЕНЫ СТРОКИ СТАТУСА ПОРТА
        #endregion         
    }
}