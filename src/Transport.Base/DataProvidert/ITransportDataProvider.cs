
using System.IO;
using System.Threading;

namespace Transport.Base.DataProvidert
{
    public interface ITransportDataProvider
    {
        byte[] GetDataByte();            //сформировать буфер для отправки.
        bool SetDataByte(byte[] data);   //получить принятый буфер.

        Stream GetStream();              // получить поток отправляемых данных
        bool SetStream(Stream stream);   // получить поток принимаемых данных

        string GetString();              // получить данные в строковом виде
        bool SetString(Stream stream);   // принять данные в строковом виде

        int CountGetDataByte { get; }    //кол-во байт для отправки.
        int CountSetDataByte { get; }    //кол-во байт для приема.

        CancellationTokenSource Cts { get; set; }
    }
}
