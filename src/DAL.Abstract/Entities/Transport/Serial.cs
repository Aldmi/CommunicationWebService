namespace DAL.Abstract.Entities.Transport
{
    public class Serial : EntityBase
    {
        public string Port { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public int StopBits { get; set; }
        public string Parity { get; set; }
        public bool DtrEnable { get; set; }
        public bool RtsEnable { get; set; }
    }
}