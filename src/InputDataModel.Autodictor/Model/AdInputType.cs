using System.Collections.Generic;

namespace InputDataModel.Autodictor.Model
{
    public class AdInputType
    {
        public int Id { get; set; }
        public IEnumerable<string> RuleNames { get; set; }//список названий правил, которым предназначенны данные
    }
}