using System;
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


        #region ctor

        public Rule(RuleOption option)
        {
            Option = option;
        }

        #endregion




        #region prop

        public int BatchSize => Option.BatchSize;

        #endregion




        #region Filter

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


        /// <summary>
        /// Проверка обрабатывает ли это правило команда
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool CheckCommand(Command4Device command)
        {
            var ruleName = $"Command_{command.ToString()}";  //Command_On, Command_Off, Command_Restart, Command_Clear
            return !ruleName.Equals("Command_None");
        }

        #endregion


        #region OrderBy

        

        #endregion

        #region StringRequest

        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из одного батча данных.
        /// </summary>
        /// <returns></returns>
        public string CreateStringRequest(IEnumerable<AdInputType> inputTypes, int startRow)
        {
            //throw new NotImplementedException("CreateStringRequest ExceptionTest");//DEBUG
            return "formatString";
        }


        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из команды.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string CreateStringRequest(Command4Device command)
        {
            return "Body"; //Option.RequestOption.Body;
        }

        #endregion
    }
}