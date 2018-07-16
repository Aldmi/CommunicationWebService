namespace DAL.Abstract.Entities.Exchange
{
    public class Request : EntityBase
    {
        public int MaxLenght { get; set; }
        public string Body { get; set; }
    }
}