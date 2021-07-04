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
            int gqrIndex = (int)((opcode >> 11) & 7);

            if ((address & 0x400) != 0)
            {
                address |= 0xFFFF_F800;
            }

            if (regBase != 0)
            {
                address += State.GPR[regBase];
            }

            // TODO: unify this logic in all quantized load opcodes. 
            uint gqr = State.SPR[912 + gqrIndex];
            int loadScale = (int)((gqr >> 2) & 0x3F);
            int loadType = (int)((gqr >> 13) & 7);

            State.FPR[regDst] = LoadQuantized(ref address, loadScale, loadType);

            if ((opcode & 0x8000) == 0)
            {
                State.FPR[regDst] |= (ulong)LoadQuantized(ref address, loadScale, loadType) << 32;
            }
            else
            {
                State.FPR[regDst] |= (ulong)BitConverter.SingleToInt32Bits(1) << 32;
            }

            State.PC += sizeof(uint);
        }
    }
}
