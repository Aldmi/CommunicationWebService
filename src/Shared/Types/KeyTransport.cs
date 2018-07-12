using System;
using Shared.Enums;

namespace Shared.Types
{
    public class KeyTransport : IEquatable<KeyTransport>
    {
        #region prop

        public string Key { get; set; }
        public TransportType TransportType { get; set; }

        #endregion



        #region prop

        public KeyTransport(string key, TransportType transportType )
        {
            Key = key;
            TransportType = transportType;
        }

        #endregion



        #region IEquatable Members

        public bool Equals(KeyTransport other)
        {
            return other != null && ((Key == other.Key) && (other.TransportType == TransportType));
        }

        #endregion
    }
}