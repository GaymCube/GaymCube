using GaymCube.Util;

namespace GaymCube.CPU
{
    partial class Gekko
    {
        private void ADDIS(uint opcode)
        {
            uint imm = opcode & 0xFFFF;
            uint src = (opcode >> 16) & 0x1F;
            uint dst = (opcode >> 21) & 0x1F;

            // TODO: what is the best way to sign-extend in C#?
            if ((imm & 0x8000) != 0)
            {
                imm |= 0xFFFF0000;
            }

            if (src == 0)
            {
                State.GPR[dst] = imm;
            }
            else
            {
                State.GPR[dst] = State.GPR[src] + imm;
            }

            State.PC += sizeof(uint);
        }

        private void ORI(uint opcode)
        {
            uint imm = opcode & 0xFFFF;
            uint dst = (opcode >> 16) & 0x1F;
            uint src = (opcode >> 21) & 0x1F;

            State.GPR[dst] = State.GPR[src] | imm;
            State.PC += sizeof(uint);
        }

        // TODO: this is very similar to the unshifted variant.
        private void ORIS(uint opcode)
        {
            uint imm = opcode & 0xFFFF;
            uint dst = (opcode >> 16) & 0x1F;
            uint src = (opcode >> 21) & 0x1F;

            State.GPR[dst] = State.GPR[src] | (imm << 16);
            State.PC += sizeof(uint);
        }

        // Rotate Left Word Immediate then AND with Mask
        private void RLWINM(uint opcode)
        {
            bool Rc = (opcode & 1) != 0;
            uint mask_extend = (opcode >> 1) & 0x1F;
            uint mask_base = (opcode >> 6) & 0x1F;
            uint shift = (opcode >> 11) & 0x1F;
            uint dst = (opcode >> 16) & 0x1F;
            uint src = (opcode >> 21) & 0x1F;

            uint mask;

            if (mask_extend == 31)
            {
                mask = 0xFFFFFFFF;
            }
            else
            {
                mask = ~(0xFFFFFFFF >> (int)(mask_extend + 1));
            }

            State.GPR[dst] = BitUtil.RotateLeft(State.GPR[src], (int)shift) & (mask >> (int)mask_base);

            if (Rc)
            {
                UpdateCR0(State.GPR[dst]);
            }

            State.PC += sizeof(uint);
        }
    }
}
