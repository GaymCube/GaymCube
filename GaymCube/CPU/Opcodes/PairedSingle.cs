using System;

namespace GaymCube.CPU
{
    partial class Gekko
    {
        private void PairedSingleQuantizedLoad(uint opcode)
        {
            uint regBase = (opcode >> 16) & 0x1F;
            uint regDst = (opcode >> 21) & 0x1F;
            uint address = opcode & 0x7FF;
            uint gqrIndex = (opcode >> 11) & 7;

            if ((address & 0x400) != 0)
            {
                address |= 0xFFFF_F800;
            }

            if (regBase != 0)
            {
                address += State.GPR[regBase];
            }

            uint graphicsQuantizationRegister = State.SPR[912 + gqrIndex];
            uint loadScale = (graphicsQuantizationRegister >> 2) & 0x3F;
            uint loadType = (graphicsQuantizationRegister >> 13) & 7;

            // We only support simple single-precision load with no scale at the moment.
            // See 2.1.2.11 Graphics Quantization Registers (GQRs) in PowerPC 750CL manual. 
            if (loadScale != 0 || loadType != 0)
            {
                throw new UnhandledOpcodeException(opcode);
            }

            ulong result;

            if ((opcode & 0x8000) == 0)
            {
                // TODO: optimize to u64 read if address is 8-byte aligned
                result = Memory.ReadWord(address);
                result |= (ulong)Memory.ReadWord(address + sizeof(uint)) << 32;
            }
            else
            {
                result = Memory.ReadWord(address);
                result |= (ulong)BitConverter.ToUInt32(BitConverter.GetBytes(1.0f)) << 32;
            }

            State.FPR[regDst] = result;
            State.PC += sizeof(uint);
        }
    }
}
