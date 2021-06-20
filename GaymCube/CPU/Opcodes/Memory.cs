using System.Diagnostics;

namespace GaymCube.CPU
{
    partial class Gekko
    {
        private void LoadWordAndZero(uint opcode)
        {
            uint address = opcode & 0xFFFF;
            uint src = (opcode >> 16) & 0x1F;
            uint dst = (opcode >> 21) & 0x1F;

            if ((address & 0x8000) != 0)
            {
                address |= 0xFFFF_0000;
            }

            if (src != 0)
            {
                address += State.GPR[src];
            }

            State.GPR[dst] = Memory.ReadWord(address);

            State.PC += sizeof(uint);
        }

        private void StoreWord(uint opcode)
        {
            uint address = opcode & 0xFFFF;
            uint dst = (opcode >> 16) & 0x1F;
            uint src = (opcode >> 21) & 0x1F;

            if ((address & 0x8000) != 0)
            {
                address |= 0xFFFF_0000;
            }

            if (dst != 0)
            {
                address += State.GPR[dst];
            }

            Memory.WriteWord(address, State.GPR[src]);

            State.PC += sizeof(uint);
        }

        private void StoreWordUpdate(uint opcode)
        {
            uint address = opcode & 0xFFFF;
            uint dst = (opcode >> 16) & 0x1F;
            uint src = (opcode >> 21) & 0x1F;

            Debug.Assert(dst != 0, "STWU: if rA = 0, the instruction form is invalid.");

            if ((address & 0x8000) != 0)
            {
                address |= 0xFFFF_0000;
            }

            address += State.GPR[dst];

            Memory.WriteWord(address, State.GPR[src]);

            State.GPR[dst] = address;

            State.PC += sizeof(uint);
        }
    }
}
