using System.Collections.Generic;

namespace DAL.EFCore.Entities.Exchange.ProvidersOption
{
    public class EfByRulesProviderOption
    {
        public List<EfRuleOption> Rules { get; set; }
    }


    public class EfRuleOption
    {
        public string Name { get; set; }                        
        public string WhereFilter { get; set; }            
        public string OrderBy { get; set; }                 
        public int TakeItems { get; set; }                               
        public List<EfViewRuleOption> ViewRules { get; set; }  
    }


    public class EfViewRuleOption
    {
        public int Id { get; set; }
        public int StartPosition { get; set; }               
        public int EndPosition { get; set; }     
        public int BatchSize { get; set; }    
        public EfRequestOption RequestOption { get; set; }     
        public EfResponseOption ResponseOption { get; set; }   
    }


    public class EfRequestOption
    {
        public string Format { get; set; }
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }


    public class EfResponseOption
    {
        public string Format { get; set; }
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }
}