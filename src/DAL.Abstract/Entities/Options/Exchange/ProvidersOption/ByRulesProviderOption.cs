using System.Collections.Generic;

namespace DAL.Abstract.Entities.Options.Exchange.ProvidersOption
{
    public class ByRulesProviderOption
    {
        public List<RuleOption> Rules { get; set; }
    }


    public class RuleOption
    {
        public string Name { get; set; }                    //Имя правила, или название команды вида Command_On, Command_Off, Command_Restart, Command_Clear
        public int BatchSize { get; set; }
        //TODO: добавить Contrains. В нем фильровать элементы по правилу, Брать N первых, N последних
        public RequestOption RequestOption { get; set; }
        public ResponseOption ResponseOption { get; set; }
    }


    public class RequestOption
    {
        public string Format { get; set; }
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }


    public class ResponseOption
    {
        public string Format { get; set; }
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }

}