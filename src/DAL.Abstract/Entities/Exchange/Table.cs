namespace DAL.Abstract.Entities.Exchange
{
    public class Table : EntityBase
    {
        public int Size { get; set; }
        public int Position { get; set; }
        public Rule Rule { get; set; }
    }
}