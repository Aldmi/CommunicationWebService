﻿namespace DAL.Abstract.Entities.Exchange
{
    public class Rule : EntityBase
    {
        public string Format { get; set; }
        public Request Request { get; set; }
        public Request Response { get; set; }
    }

}