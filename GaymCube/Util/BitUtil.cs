using System;

namespace GaymCube.Util
{
    static class BitUtil
    {
        public static uint RotateLeft(uint value, int amount)
        {
            if (amount != 0)
            {
                return (value << amount) | (value >> (32 - amount));
            }
            return value;
        }
    }
}
