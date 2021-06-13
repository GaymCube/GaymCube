using System;

namespace GaymCube.CPU
{
    partial class Gekko
    {
        private void MoveFromMSR(uint opcode)
        {
            uint dst = (opcode >> 21) & 0x1F;
            State.GPR[dst] = State.MSR;
            State.PC += sizeof(uint);
        }

        private void MoveFromSPR(uint opcode)
        {
            uint spr_hi = (opcode >> 11) & 0x1F;
            uint spr_lo = (opcode >> 16) & 0x1F;
            uint src = (spr_hi << 5) | spr_lo;
            uint dst = (opcode >> 21) & 0x1F;

            State.GPR[dst] = State.SPR[src];
            State.PC += sizeof(uint);
        }

        private void MoveToSPR(uint opcode)
        {
            uint spr_hi = (opcode >> 11) & 0x1F;
            uint spr_lo = (opcode >> 16) & 0x1F;
            uint dst = (spr_hi << 5) | spr_lo;
            uint src = (opcode >> 21) & 0x1F;

            State.SPR[dst] = State.GPR[src];
            State.PC += sizeof(uint);
        }
    }
}
