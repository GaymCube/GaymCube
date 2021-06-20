using System;

namespace GaymCube.CPU
{
    partial class Gekko
    {
        private void Branch(uint opcode)
        {
            bool link = (opcode & 1) != 0;
            bool absolute = (opcode & 2) != 0;
            uint address = opcode & 0x3FF_FFFC;

            // TODO: what is the best way to sign-extend in C#?
            if ((address & 0x200_0000) != 0)
            {
                address |= 0xFC00_0000;
            }

            if (link)
            {
                State.LR = State.PC + sizeof(uint);
            }

            if (absolute)
            {
                State.PC = address;
            }
            else
            {
                State.PC = State.PC + address;
            }
        }

        private void BranchConditionalToLR(uint opcode)
        {
            bool link = (opcode & 1) != 0;
            int conditionIndex = (int)((opcode >> 16) & 0x1F);
            bool forceCounterOK = (opcode & (1 << 23)) != 0;
            bool forceConditionOK = (opcode & (1 << 25)) != 0;

            bool counterOK = true;
            bool conditionOK = true;

            if (!forceCounterOK)
            {
                bool invertCounterOK = (opcode & (1 << 22)) != 0;

                counterOK = --State.CTR != 0;

                if (invertCounterOK)
                {
                    counterOK = !counterOK;
                }
            }

            if (!forceConditionOK)
            {
                bool invertConditionOK = (opcode & (1 << 24)) == 0;

                conditionOK = (State.CR0 & (0x8000_0000 >> conditionIndex)) != 0;

                if (invertConditionOK)
                {
                    conditionOK = !conditionOK;
                }
            }

            if (counterOK && conditionOK)
            {
                uint lr = State.LR & 0xFFFF_FFFC;
                if (link)
                {
                    State.LR = State.PC + sizeof(uint);
                }
                State.PC = lr;
            }
            else
            {
                State.PC += sizeof(uint);
            }
        }

        private void ReturnFromInterrupt()
        {
            const uint MSR_MASK = 0x87C0_FFF3;

            State.MSR &= ~(MSR_MASK | 0x4_0000);
            State.MSR |= State.SRR1 & MSR_MASK;
            State.PC = State.SRR0 & 0xFFFF_FFFC;
        }
    }
}
