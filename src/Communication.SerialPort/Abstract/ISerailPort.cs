using System;
using Transport.SerialPort.Option;

namespace Transport.SerialPort.Abstract
{
    public interface ISerailPort : IDisposable
    {
        SerialOption SerialOption { get; }
        void Send(byte[] data);
        bool Recive();
    }
}