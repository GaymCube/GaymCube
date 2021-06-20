using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using GaymCube.CPU;

namespace GaymCube
{
    class Memory : IMemory
    {
        class UnhandledAddressException : Exception
        {
            public UnhandledAddressException(uint address)
                : base(string.Format("Memory access to unknown address: 0x{0:X8}", address))
            {
            }
        }

        private const uint MainMemorySize = 0x0180_0000;

        private const uint MainMemoryMirror0StartAddress = 0x0000_0000;
        private const uint MainMemoryMirror1StartAddress = 0x8000_0000;
        private const uint MainMemoryMirror2StartAddress = 0xC000_0000;

        private const uint MainMemoryMirror0EndAddress = MainMemoryMirror0StartAddress + MainMemorySize - 1;
        private const uint MainMemoryMirror1EndAddress = MainMemoryMirror1StartAddress + MainMemorySize - 1;
        private const uint MainMemoryMirror2EndAddress = MainMemoryMirror2StartAddress + MainMemorySize - 1;

        private byte[] _mainMemory = new byte[MainMemorySize];

        public Memory()
        {
            _mainMemory.AsSpan().Fill(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsInsideMainMemory(uint address)
        {
            return (address >= MainMemoryMirror0StartAddress && address <= MainMemoryMirror0EndAddress) ||
               (address >= MainMemoryMirror1StartAddress && address <= MainMemoryMirror1EndAddress) ||
               (address >= MainMemoryMirror2StartAddress && address <= MainMemoryMirror2EndAddress);
        }

        // TODO: think of a good way to decode addresses.
        // TODO: should addresses be force-aligned?
        // TODO: can we fake big-endian without BSwap?

        public byte ReadByte(uint address)
        {
            return GetSpan(address, sizeof(ushort))[0];
        }

        public ushort ReadHalf(uint address)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(GetSpan(address, sizeof(ushort)));
        }

        public uint ReadWord(uint address)
        {
            return BinaryPrimitives.ReadUInt32BigEndian(GetSpan(address, sizeof(uint)));
        }

        public void WriteByte(uint address, byte value)
        {
            GetSpan(address, sizeof(ushort))[0] = value;
        }

        public void WriteHalf(uint address, ushort value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(GetSpan(address, sizeof(ushort)), value);
        }

        public void WriteWord(uint address, uint value)
        {
            BinaryPrimitives.WriteUInt32BigEndian(GetSpan(address, sizeof(uint)), value);
        }

        public Span<byte> GetSpan(uint address, int size)
        {
            if (IsInsideMainMemory(address))
            {
                int offset = (int)(address & (MainMemorySize - 1));

                return _mainMemory.AsSpan().Slice(offset, size);
            }

            throw new NotImplementedException();
        }
    }
}
