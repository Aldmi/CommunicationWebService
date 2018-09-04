using System.Collections.Generic;

namespace InputDataModel.Autodictor.InputData
{
    public class UniversalInputType
    {
        public int Id { get; set; }
        public IEnumerable<string> RuleNames { get; set; }//список названий правил, которым предназначенны данные
    }
}