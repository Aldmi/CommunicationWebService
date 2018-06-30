using Communication.SerialPort.Abstract;
using Communication.SerialPort.Option;

namespace Communication.SerialPort.Concrete.Sp4Win
{
    public class SpWinDefault : ISerailPort
    {
        #region ctor

        public SpWinDefault(SerialOption option)
        {
            SerialOption = option;
        }

        #endregion




        #region prop

        public SerialOption SerialOption { get; }

        #endregion




        public void Send(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public bool Recive()
        {
            throw new System.NotImplementedException();
        }


        public void Dispose()
        {
          
        }
    }
}