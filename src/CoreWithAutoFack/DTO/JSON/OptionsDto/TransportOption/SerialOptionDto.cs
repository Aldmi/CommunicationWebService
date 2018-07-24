using DAL.Abstract.Entities.Transport;

namespace WebServer.DTO.JSON.OptionsDto.TransportOption
{
    public class SerialOptionDto
    {
        public int Id { get; set; }
        public string Port { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
        public bool DtrEnable { get; set; }
        public bool RtsEnable { get; set; }
    }
}