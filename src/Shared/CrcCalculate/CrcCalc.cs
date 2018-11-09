using System.Collections.Generic;

namespace Shared.CrcCalculate
{
    public static class CrcCalc
    {
        public static byte CalcXor(IReadOnlyList<byte> arr)
        {
            var xor = arr[0];
            for (var i = 1; i < arr.Count; i++)
            {
                xor ^= arr[i];
            }
            xor ^= 0xFF;

            return xor;
        }
    }
}