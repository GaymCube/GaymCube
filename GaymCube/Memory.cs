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

        private const uint MainMemoryStartAddress = 0x8000_0000;
        private const uint MainMemoryEndAddress = 0x817F_FFFF;
        private const uint MainMemorySize = MainMemoryEndAddress - MainMemoryStartAddress + 1;

        private byte[] _mainMemory = new byte[MainMemorySize];

        public Memory()
        {
            _mainMemory.AsSpan().Fill(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsInsideMainMemory(uint address)
        {
            return address >= MainMemoryStartAddress && address <= MainMemoryEndAddress;
        }

        // TODO: think of a good way to decode addresses.
        // TODO: should addresses be force-aligned?
        // TODO: can we fake big-endian without BSwap?

        public byte ReadByte(uint address)
        {
            if (IsInsideMainMemory(address))
            {
                return GetSpan(address, sizeof(ushort))[0];
            }

            throw new UnhandledAddressException(address);
        }

        public ushort ReadHalf(uint address)
        {
            if (IsInsideMainMemory(address))
            {
                return BinaryPrimitives.ReadUInt16BigEndian(GetSpan(address, sizeof(ushort)));
            }

            throw new UnhandledAddressException(address);
        }

        public uint ReadWord(uint address)
        {
            if (IsInsideMainMemory(address))
            {
                return BinaryPrimitives.ReadUInt32BigEndian(GetSpan(address, sizeof(uint)));
            }

            throw new UnhandledAddressException(address);
        }

        public void WriteByte(uint address, byte value)
        {
            if (IsInsideMainMemory(address))
            {
                GetSpan(address, sizeof(ushort))[0] = value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void WriteHalf(uint address, ushort value)
        {
            if (IsInsideMainMemory(address))
            {
                BinaryPrimitives.WriteUInt16BigEndian(GetSpan(address, sizeof(ushort)), value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void WriteWord(uint address, uint value)
        {
            if (IsInsideMainMemory(address))
            {
                BinaryPrimitives.WriteUInt32BigEndian(GetSpan(address, sizeof(uint)), value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Span<byte> GetSpan(uint address, int size)
        {
            if (IsInsideMainMemory(address))
            {
                int offset = (int)(address & 0x017F_FFFF);

                return _mainMemory.AsSpan().Slice(offset, size);
            }

            throw new NotImplementedException();
        }
    }
}
