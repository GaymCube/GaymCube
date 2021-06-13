using System;

namespace GaymCube.CPU
{
    interface IMemory
    {
        Span<byte> GetSpan(uint address, int size);

        byte ReadByte(uint address);
        ushort ReadHalf(uint address);
        uint ReadWord(uint address);
        void WriteByte(uint address, byte value);
        void WriteHalf(uint address, ushort value);
        void WriteWord(uint address, uint value);
    }
}
