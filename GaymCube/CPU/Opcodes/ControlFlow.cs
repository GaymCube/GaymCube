﻿using System;

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

        private void ReturnFromInterrupt()
        {
            const uint MSR_MASK = 0x87C0_FFF3;

            State.MSR &= ~(MSR_MASK | 0x4_0000);
            State.MSR |= State.SRR1 & MSR_MASK;
            State.PC = State.SRR0 & 0xFFFF_FFFC;
        }
    }
}
