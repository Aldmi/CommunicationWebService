namespace DAL.Abstract.Entities.Options.Exchange
{
    public class Response : EntityBase
    {
        public int MaxLenght { get; set; }
        public int TimeRespone { get; set; }
        public string Body { get; set; }
    }
}