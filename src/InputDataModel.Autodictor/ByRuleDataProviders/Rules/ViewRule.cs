using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;

namespace InputDataModel.Autodictor.ByRuleDataProviders.Rules
{
    public class ViewRule
    {
        #region fields

        public readonly ViewRuleOption Option;

        #endregion



        #region ctor

        public ViewRule(ViewRuleOption option)
        {
            Option = option;
        }

        #endregion




    }
}