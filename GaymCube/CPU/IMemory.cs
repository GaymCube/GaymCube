namespace GaymCube.CPU
{
    interface IMemory
    {
        byte ReadByte(uint address);
        ushort ReadHalf(uint address);
        uint ReadWord(uint address);
        void WriteByte(uint address, byte value);
        void WriteHalf(uint address, ushort value);
        void WriteWord(uint address, uint value);
    }
}
