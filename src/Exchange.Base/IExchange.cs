using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Exchange.Base.Model;
using Exchange.Base.RxModel;
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
        bool IsStartedCycleExchange { get; }                                      //Флаг цикл. обмена
        InDataWrapper<T> LastSendData { get; }                                    //Последние отосланные данные 
        #endregion


        #region StartStop
        Task CycleReOpened();
        void CycleReOpenedCancelation();
        void StartCycleExchange();                                                             //Запустить цикл. обмен (ДОБАВИТЬ функцию цикл обмена на бекграунд)
        void StopCycleExchange();                                                              //Остановить цикл. обмен (УДАЛИТЬ функцию цикл обмена из бекграунд)
        #endregion


        #region SendData
        void SendCommand(Command4Device command);                              //однократно выполняемая команда
        void SendOneTimeData(IEnumerable<T> inData);                           //однократно отсылаемые данные (если указанны правила, то только для этих правил)
        void SendCycleTimeData(IEnumerable<T> inData);                         //циклически отсылаемые данные
        #endregion


        #region ExchangeRx
        ISubject<ConnectChangeRxModel> IsConnectChangeRx { get; }                                       //СОБЫТИЕ СМЕНЫ КОННЕКТА IsConnect. МЕНЯЕТСЯ В ПРОЦЕССЕ ОБМЕНА.
        ISubject<LastSendDataChangeRxModel<T>> LastSendDataChangeRx { get; }                            //СОБЫТИЕ ИЗМЕНЕНИЯ ПОСЛЕД ОТПРАВЕЛННЫХ ДАННЫХ LastSendData.
        ISubject<ResponsePieceOfDataWrapper<T>> ResponseChangeRx { get; }                                   //СОБЫТИЕ ОТВЕТА НА ПЕРЕДАННЫЮ ПОРЦИЮ ДАННЫХ. 
        #endregion


        #region TransportRx
        ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx { get; }                                  //ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. ОТКРЫТИЯ/ЗАКРЫТИЯ ПОДКЛЮЧЕНИЯ.
        ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx { get; }          //ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. СМЕНЫ СОСТОЯНИЯ СТАТУСА ОБМЕНА ДАННЫМИ.
        ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx { get; }                      //ПРОКИНУТОЕ СОБЫТИЕ ТРАНСПОРТА. СМЕНЫ СТРОКИ СТАТУСА ПОРТА.
        #endregion         
    }
}