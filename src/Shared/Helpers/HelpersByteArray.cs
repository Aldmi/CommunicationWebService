﻿using System.Linq;
using System.Text;

namespace Shared.Helpers
{
    public static class HelpersByteArray
    {
        public static byte[] ConvertString2ByteArray(this string str, string format)
        {
            byte[] resultBuffer = null;
            switch (format)
            {
                //Распарсить строку в масив байт как она есть. 0203АА96 ...
                case "HEX":
                      break;

                 default:
                    resultBuffer = Encoding.GetEncoding(format).GetBytes(str).ToArray();
                    break;
            }
            return resultBuffer;
        }
    }
}