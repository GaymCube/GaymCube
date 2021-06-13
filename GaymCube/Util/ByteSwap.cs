using System;

namespace GaymCube.Util
{
    static class ByteSwap
    {
        public static uint Swap32(uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data);
        }

        public static ushort Swap16(ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data);
        }
    }
}
