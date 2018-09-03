using DAL.Abstract.Entities.Options.Transport;
using Transport.Base.Abstract;


namespace Transport.SerialPort.Abstract
{
    public interface ISerailPort : ITransport
    {
        SerialOption Option { get; }                                                           //НАСТРОЙКИ ПОРТА
    }
}