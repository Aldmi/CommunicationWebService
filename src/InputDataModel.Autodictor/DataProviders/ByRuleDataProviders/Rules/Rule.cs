using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using InputDataModel.Autodictor.Model;
using InputDataModel.Base;

namespace InputDataModel.Autodictor.DataProviders.ByRuleDataProviders.Rules
{
    public class Rule
    {
        #region fields

        public readonly RuleOption Option;

        #endregion



        #region ctor

        public Rule(RuleOption option)
        {
            Option = option;
            ViewRules= option.ViewRules.Select(opt=> new ViewRule(Option.AddressDevice, opt)).ToList();
        }

        #endregion



        #region prop

        public IEnumerable<ViewRule> ViewRules { get; set; }

        #endregion




        #region Filter

        /// <summary>
        /// Проверка обрабатывает ли это правило команда
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool IsCommand(Command4Device command)
        {
            var ruleName = $"Command_{command.ToString()}";  //Command_On, Command_Off, Command_Restart, Command_Clear
            return !ruleName.Equals("Command_None");
        }

        #endregion




        #region StringRequest

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