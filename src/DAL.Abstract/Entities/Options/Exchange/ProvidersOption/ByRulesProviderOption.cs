using System.Collections.Generic;

namespace DAL.Abstract.Entities.Options.Exchange.ProvidersOption
{
    public class ByRulesProviderOption
    {
        public List<Rule> Rules { get; set; }
    }


    public class Rule
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }


    public class Request
    {
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }


    public class Response
    {
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }

}