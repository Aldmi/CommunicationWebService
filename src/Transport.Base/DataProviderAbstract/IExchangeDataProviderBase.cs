
using System.IO;

namespace Transport.Base.DataProviderAbstract
{
    public interface IExchangeDataProviderBase
    {
        byte[] GetDataByte();            //сформировать буфер для отправки.
        bool SetDataByte(byte[] data);   //получить принятый буфер.

        Stream GetStream();              // получить поток отправляемых данных
        bool SetStream(Stream stream);   // получить поток принимаемых данных


        int CountGetDataByte { get; }    //кол-во байт для отправки.
        int CountSetDataByte { get; }    //кол-во байт для приема.
    }
}
