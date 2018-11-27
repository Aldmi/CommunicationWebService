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



        #region StringRequest

        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из команды.
        /// </summary>
        /// <returns></returns>
        public ViewRuleRequestModelWrapper GetRequestString()
        {
            var commandViewRule = ViewRules.FirstOrDefault();
            return commandViewRule?.GetCommandRequestString();
        }

        #endregion
    }
}