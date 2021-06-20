using System.Numerics;

namespace GaymCube.CPU
{
    partial class Gekko
    {
        private void ADDI(uint opcode)
        {
            uint imm = (opcode & 0xFFFF) << 16;
            uint src = (opcode >> 16) & 0x1F;
            uint dst = (opcode >> 21) & 0x1F;

            if ((imm & 0x8000) != 0)
            {
                imm |= 0xFFFF_0000;
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

        // TODO: this is very similar to the unshifted variant.
        private void ADDIS(uint opcode)
        {
            uint imm = (opcode & 0xFFFF) << 16;
            uint src = (opcode >> 16) & 0x1F;
            uint dst = (opcode >> 21) & 0x1F;

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
            uint imm = (opcode & 0xFFFF) << 16;
            uint dst = (opcode >> 16) & 0x1F;
            uint src = (opcode >> 21) & 0x1F;

            State.GPR[dst] = State.GPR[src] | imm;
            State.PC += sizeof(uint);
        }

        // Rotate Left Word Immediate then AND with Mask
        private void RLWINM(uint opcode)
        {
            bool Rc = (opcode & 1) != 0;
            int mask_end = (int)((opcode >> 1) & 0x1F);
            int mask_base = (int)((opcode >> 6) & 0x1F);
            int shift = (int)((opcode >> 11) & 0x1F);
            uint dst = (opcode >> 16) & 0x1F;
            uint src = (opcode >> 21) & 0x1F;

            uint value = BitOperations.RotateLeft(State.GPR[src], shift);

            if (mask_base <= mask_end)
            {
                int length = mask_end - mask_base + 1;
                value &= ~(0xFFFFFFFF << length) << (31 - mask_end);
            }
            else
            {
                int length = mask_base - mask_end + 1;
                value &= ~(~(0xFFFFFFFF << length) << (31 - mask_base));
            }

            if (Rc)
            {
                UpdateCR0(value);
            }


            State.GPR[dst] = value;
            State.PC += sizeof(uint);
        }
    }
}
