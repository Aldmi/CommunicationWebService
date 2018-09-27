using System.Collections.Generic;

namespace DAL.Abstract.Entities.Options.Exchange.ProvidersOption
{
    public class ByRulesProviderOption
    {
        public List<RuleOption> Rules { get; set; }
    }


    public class RuleOption
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public int BachSize { get; set; }
        public RequestOption RequestOption { get; set; }
        public ResponseOption ResponseOption { get; set; }
    }


    public class RequestOption
    {
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }


    public class ResponseOption
    {
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }

}