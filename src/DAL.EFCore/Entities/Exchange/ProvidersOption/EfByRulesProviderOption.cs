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
        public string AddressDevice { get; set; }       
        public string WhereFilter { get; set; }            
        public string OrderBy { get; set; }                 
        public int TakeItems { get; set; }       
        public string DefaultItemJson { get; set; } 
        public List<EfViewRuleOption> ViewRules { get; set; }  
    }


    public class EfViewRuleOption
    {
        public int Id { get; set; }
        public int StartPosition { get; set; }               
        public int Count { get; set; }     
        public int BatchSize { get; set; }    
        public EfRequestOption RequestOption { get; set; }     
        public EfResponseOption ResponseOption { get; set; }   
    }


    public class EfRequestOption
    {
        public string Format { get; set; }
        public int MaxBodyLenght { get; set; }
        public string Header { get; set; }                   // НАЧАЛО запроса (ТОЛЬКО ЗАВИСИМЫЕ ДАННЫЕ).
        public string Body { get; set; }                     // ТЕЛО запроса (ТОЛЬКО НЕЗАВИСИМЫЕ ДАННЫЕ). Каждый элемент батча подставляет свои данные в Body, затем все элементы Конкатенируются.
        public string Footer { get; set; }                   // КОНЕЦ ЗАПРОСА (ТОЛЬКО ЗАВИСИМЫЕ ДАННЫЕ).
    }


    public class EfResponseOption
    {
        public string Format { get; set; }
        public int Lenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }
}