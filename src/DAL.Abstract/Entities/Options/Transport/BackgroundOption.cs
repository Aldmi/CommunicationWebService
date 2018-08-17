namespace DAL.Abstract.Entities.Options.Transport
{
    public class BackgroundOption : EntityBase
    {
        public bool AutoStart { get; set; }      //Авто старт бекграунда для данного транспорта
    }
}