using System.Collections.Generic;
using System.Linq;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using InputDataModel.Autodictor.Model;
using InputDataModel.Base;

namespace InputDataModel.Autodictor.ByRuleDataProviders.Rules
{

    public class Rule
    {
        public readonly RuleOption Option;

        public Rule(RuleOption option)
        {
            Option = option;
        }


        public int BatchSize => Option.BatchSize;




        /// <summary>
        /// Фильтровать элементы по Contrains этого правила.
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        public IEnumerable<AdInputType> FilterItems(IEnumerable<AdInputType> inData)
        {
            if(inData == null)
                return new List<AdInputType>();

            var chekedItems = inData.Where(CheckItem);

           
            return chekedItems;
        }


        /// <summary>
        /// Проверяет элемент под ограничения правила.
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        private bool CheckItem(AdInputType inputType)
        {
            //TODO: проверить Contrains для этого элемента

            return true;
        }


        public bool CheckCommand(Command4Device command)
        {
            return true;
        }


  

        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из одного батча данных.
        /// </summary>
        /// <returns></returns>
        public string CreateStringRequest(IEnumerable<AdInputType> inputTypes, int startRow)
        {
            //throw new NotImplementedException("ddsfdsf");//DEBUG
            return "formatString";
        }


        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из команды.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string CreateStringRequest(Command4Device command)
        {
            return Option.RequestOption.Body;
        }
    }
}