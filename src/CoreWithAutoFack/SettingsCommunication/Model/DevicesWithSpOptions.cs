using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace CoreWithAutoFack.SettingsCommunication.Model
{
    public class DevicesWithSpOptions
    {
        public List<DeviceSp> DevicesSp { get; set; }
    }


    public class DeviceSp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Port { get; set; }
        public string Address { get; set; }
        public int TimeRespone { get; set; }
        public string Description { get; set; }
        public string Binding { get; set; }
        public ExchangeRule ExchangeRules { get; set; }
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
        public Request Request { get; set; }
        public Request Response { get; set; }
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