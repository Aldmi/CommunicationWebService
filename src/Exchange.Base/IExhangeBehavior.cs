using System.Collections.Generic;
using Exchange.Base.Model;
using Shared.Enums;

namespace Exchange.Base
{
    /// <summary>
    /// УНИВЕРСАЛЬНОЕ ОБМЕН ДАННЫМИ СО ВСЕМИ УСТРОЙСТВАМИ.
    /// </summary>
    public interface IExhangeBehavior
    {
        string TransportName { get; }                                              //Название порта или адресс ус-ва 
        UniversalInputType LastSendData { get; }                                  //Последние отосланные данные
        IEnumerable<string> GetRuleNames { get; }                                 //Отдать название установленных правил обмена
        TypeExchange TypeExchange { get; }                                        //Тип обмена

        void StartCycleExchange();                                                //Запустить цикл. обмен (ДОБАВИТЬ функцию цикл обмена на бекграунд)
        void StopCycleExchange();                                                 //Остановить цикл. обмен (УДАЛИТЬ функцию цикл обмена из бекграунд)

        void SendCommand(string commandName, UniversalInputType data4Command = null);           //однократно выполняемая команда
        void SendOneTimeData(UniversalInputType inData);                                        //однократно отсылаемые данные (если указанны правила, то только для этих правил)
        void SendCycleTimeData(UniversalInputType inData);                                      //циклически отсылаемые данные
    }
}