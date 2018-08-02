namespace DAL.Abstract.Entities.Options.Exchange
{
    public class Request : EntityBase
    {
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }
}