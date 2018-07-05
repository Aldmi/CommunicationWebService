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
        string TransportName { get; set; }                                        //Название порта или адресс ус-ва 
        UniversalInputType LastSendData { get; }                                 //Последние отосланные данные
        IEnumerable<string> GetRuleNames { get; }

        TypeExchange TypeExchange { get; }
        void StartCycleExchange();
        void StopCycleExchange();

        void SendCommand(string commandName, UniversalInputType data4Command = null);                    //однократно выполняемая команда
        void SendOneTimeData(UniversalInputType uit);                                                    //однократно отсылаемые данные (если указанны правила, то только для этих правил)
        void SendCycleTimeData(UniversalInputType uit);                                                  //циклически отсылаемые данные

    }
}