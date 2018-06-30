using System.Collections.Generic;

namespace Exchange.MasterSerialPort.Option
{

    public class ExchangeMasterSpOptions
    {
        public List<ExchangeMasterSpOption> ExchangesMasterSp { get; set; }
    }

    public class ExchangeMasterSpOption
    {
        public string PortName { get; set; }
        public string Address { get; set; }
        public int TimeResponse { get; set; }
        public ExchangeRule ExchangeRule { get; set; }
    }


    public class ExchangeRule
    {
        public Table Table { get; set; }
    }

    public class Table
    {
        public int Size { get; set; }
        public int Position { get; set; }
        public Rule Rule { get; set; }
    }

    public class Rule
    {
        public string Format { get; set; }
        public Description Request { get; set; }
        public Description Response { get; set; }
    }

    public class Description
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