using System;
using GaymCube.CPU;
using GaymCube.Util;

namespace GaymCube
{
    class Memory : IMemory
    {
        class UnhandledAddressException : Exception
        {
            public UnhandledAddressException(UInt32 address)
                : base(String.Format("Memory access to unknown address: 0x{0:X8}", address))
            {
            }
        }

        private Byte[] main_ram = new byte[0x180_0000];

        public Memory()
        {
            Array.Fill<Byte>(main_ram, 0);
        }

        // TODO: think of a good way to decode addresses.
        // TODO: should addresses be force-aligned?
        // TODO: can we fake big-endian without BSwap?

        public unsafe Byte ReadByte(UInt32 address)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (Byte* data = main_ram)
                {
                    return *(data + offset);
                }
            }

            throw new UnhandledAddressException(address);
        }

        public unsafe UInt16 ReadHalf(UInt32 address)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (Byte* data = main_ram)
                {
                    return ByteSwap.Swap16(*(UInt16*)(data + offset));
                }
            }

            throw new UnhandledAddressException(address);
        }

        public unsafe UInt32 ReadWord(UInt32 address)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (Byte* data = main_ram)
                {
                    return ByteSwap.Swap32(*(UInt32*)(data + offset));
                }
            }

            throw new UnhandledAddressException(address);
        }

        public unsafe void WriteByte(UInt32 address, Byte value)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (Byte* data = main_ram)
                {
                    *(data + offset) = value;
                    return;
                }
            }

            throw new NotImplementedException();
        }

        public unsafe void WriteHalf(UInt32 address, UInt16 value)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (Byte* data = main_ram)
                {
                    *(UInt16*)(data + offset) = ByteSwap.Swap16(value);
                    return;
                }
            }

            throw new NotImplementedException();
        }

        public unsafe void WriteWord(UInt32 address, UInt32 value)
        {
            if (address >= 0x8000_0000 && address <= 0x817F_FFFF)
            {
                var offset = address & 0x017F_FFFF;

                fixed (Byte* data = main_ram)
                {
                    *(UInt32*)(data + offset) = ByteSwap.Swap32(value);
                    return;
                }
            }

            throw new NotImplementedException();
        }
    }
}
