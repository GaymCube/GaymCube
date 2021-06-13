using System;

namespace GaymCube.Util
{
    static class ByteSwap
    {
        public static UInt32 Swap32(UInt32 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data);
        }

        public static UInt16 Swap16(UInt16 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data);
        }
    }
}
