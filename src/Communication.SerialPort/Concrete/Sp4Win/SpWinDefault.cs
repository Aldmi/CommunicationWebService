using Transport.SerialPort.Abstract;
using Transport.SerialPort.Option;

namespace Transport.SerialPort.Concrete.Sp4Win
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