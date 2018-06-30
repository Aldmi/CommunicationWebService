using System;
using Communication.SerialPort.Option;

namespace Communication.SerialPort.Abstract
{
    public interface ISerailPort : IDisposable
    {
        SerialOption SerialOption { get; }
        void Send(byte[] data);
        bool Recive();
    }
}