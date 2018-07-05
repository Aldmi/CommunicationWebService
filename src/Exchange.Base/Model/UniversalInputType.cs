using System.Collections.Generic;
using System.Reflection;

namespace Exchange.Base.Model
{
    public class UniversalInputType
    {
        public int Id { get; set; }
        public IEnumerable<string> RuleNames { get; set; }//список названий правил, которым предназначенны данные
    }
}