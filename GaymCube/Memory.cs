using System;
using GaymCube.CPU;
using GaymCube.Util;

namespace GaymCube
{
    class Memory : IMemory
    {
        class UnhandledAddressException : Exception
        {
            public UnhandledAddressException(uint address)
                : base(String.Format("Memory access to unknown address: 0x{0:X8}", address))
            {
            }
        }

        private byte[] main_ram = new byte[0x180_0000];

        public Memory()
        {
            Array.Fill<byte>(main_ram, 0);
        }

        // TODO: think of a good way to decode addresses.
        // TODO: should addresses be force-aligned?
        // TODO: can we fake big-endian without BSwap?

        public unsafe byte ReadByte(uint address)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (byte* data = main_ram)
                {
                    return *(data + offset);
                }
            }

            throw new UnhandledAddressException(address);
        }

        public unsafe ushort ReadHalf(uint address)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (byte* data = main_ram)
                {
                    return ByteSwap.Swap16(*(ushort*)(data + offset));
                }
            }

            throw new UnhandledAddressException(address);
        }

        public unsafe uint ReadWord(uint address)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (byte* data = main_ram)
                {
                    return ByteSwap.Swap32(*(uint*)(data + offset));
                }
            }

            throw new UnhandledAddressException(address);
        }

        public unsafe void WriteByte(uint address, byte value)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (byte* data = main_ram)
                {
                    *(data + offset) = value;
                    return;
                }
            }

            throw new NotImplementedException();
        }

        public unsafe void WriteHalf(uint address, ushort value)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (byte* data = main_ram)
                {
                    *(ushort*)(data + offset) = ByteSwap.Swap16(value);
                    return;
                }
            }

            throw new NotImplementedException();
        }

        public unsafe void WriteWord(uint address, uint value)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (byte* data = main_ram)
                {
                    *(uint*)(data + offset) = ByteSwap.Swap32(value);
                    return;
                }
            }

            throw new NotImplementedException();
        }
    }
}
