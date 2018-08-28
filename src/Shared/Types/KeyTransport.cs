using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Types
{
    public class KeyTransport : IEquatable<KeyTransport>
    {
        #region prop

        public string Key { get; }
        public TransportType TransportType { get; }

        #endregion




        #region prop

        public KeyTransport(string key, TransportType transportType )
        {
            Key = key;
            TransportType = transportType;
        }

        public KeyTransport(Dictionary<string, string> dict)
        {
            if (!Enum.TryParse(dict["Type"], out TransportType trType))
                return;

            Key = dict["Key"];
            TransportType = trType;
        }

        #endregion



        #region IEquatable Members

        public bool Equals(KeyTransport other)
        {
            return other != null && ((Key == other.Key) && (other.TransportType == TransportType));
        }


        public override int GetHashCode()
        {
            return Key.GetHashCode() + TransportType.GetHashCode();
        }

        #endregion



        public override string ToString()
        {
            return $"TransportType= {TransportType}  Key={Key}";
        }
    }
}