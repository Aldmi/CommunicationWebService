using System.Collections.Generic;

namespace DAL.EFCore.Entities.Exchange.ProvidersOption
{
    public class EfByRulesProviderOption
    {
        public List<EfRule> Rules { get; set; }
    }


    public class EfRule
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public EfRequest Request { get; set; }
        public EfResponse Response { get; set; }
    }


    public class EfRequest
    {
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }


    public class EfResponse
    {
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }
}