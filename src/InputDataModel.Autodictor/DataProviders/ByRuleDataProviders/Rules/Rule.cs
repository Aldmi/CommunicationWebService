using System.Collections.Generic;
using System.Linq;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;

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
    }
}