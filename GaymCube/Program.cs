using System;
using System.IO;
using GaymCube.Util;
using GaymCube.CPU;

namespace GaymCube
{
    class Program
    {
        static void Main(string[] args)
        {
            Memory memory = new Memory();
            Gekko gekko = new Gekko(memory);

            Stream stream = File.OpenRead("template.dol");

            using (BinaryReader binary_reader = new BinaryReader(stream))
            {
                // Read code and data sections
                for (int i = 0; i < 18; i++)
                {
                    uint file_address;
                    uint memory_address;
                    uint size;

                    binary_reader.BaseStream.Position = 0x00 + i * sizeof(uint);
                    file_address = ByteSwap.Swap32(binary_reader.ReadUInt32());

                    binary_reader.BaseStream.Position = 0x48 + i * sizeof(uint);
                    memory_address = ByteSwap.Swap32(binary_reader.ReadUInt32());

                    binary_reader.BaseStream.Position = 0x90 + i * sizeof(uint);
                    size = ByteSwap.Swap32(binary_reader.ReadUInt32());

                    Console.WriteLine("section[{0}]:\t src=0x{1:X8} dst=0x{2:X8} size=0x{3:X8}", i, file_address, memory_address, size);

                    binary_reader.BaseStream.Position = file_address;

                    for (uint j = 0; j < size; j++)
                    {
                        memory.WriteByte(memory_address++, binary_reader.ReadByte());
                    }
                }

                uint bss_address;
                uint bss_size;
                uint entrypoint;

                binary_reader.BaseStream.Position = 0xD8;
                bss_address = ByteSwap.Swap32(binary_reader.ReadUInt32());
                bss_size = ByteSwap.Swap32(binary_reader.ReadUInt32());
                entrypoint = ByteSwap.Swap32(binary_reader.ReadUInt32());

                Console.WriteLine("BSS: address=0x{0:X8} size=0x{1:X8}", bss_address, bss_size);
                Console.WriteLine("Entrypoint: 0x{0:X8}", entrypoint);

                // Clear the BSS region
                for (uint i = 0; i < bss_size; i++)
                {
                    memory.WriteByte(bss_address++, 0);
                }

                // Setup Gekko to start execution at the entrypoint
                gekko.State.PC = entrypoint;
            }

            gekko.Run(32);
        }
    }
}
