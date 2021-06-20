using System;

namespace GaymCube.CPU
{
    class GekkoState
    {
        public enum Condition : uint
        {
            LT = 0x8000_0000,
            GT = 0x4000_0000,
            EQ = 0x2000_0000,
            SO = 0x1000_0000
        }

        // General Purpose Registers R0 - R31
        public uint[] GPR { get; } = new uint[32];

        // Special Purpose Registers SPR0 - SPR1023
        public uint[] SPR { get; } = new uint[1024];
        public uint LR
        {
            get
            {
                return SPR[8];
            }
            set
            {
                SPR[8] = value;
            }
        }
        public uint CTR
        {
            get
            {
                return SPR[9];
            }
            set
            {
                SPR[9] = value;
            }
        }
        public uint SRR0
        {
            get
            {
                return SPR[26];
            }
        }
        public uint SRR1
        {
            get
            {
                return SPR[27];
            }
        }

        // Program Counter  (PC)
        public uint PC { get; set; }

        // Condition Register 0 (CR0)
        // Bit 0: less than zero (sign-flag)
        // Bit 1: greater than zero
        // Bit 2: equal to zero (zero-flag)
        // Bit 3: summary overflow (copy of overflow flag in XER)
        public uint CR0 { get; set; }

        // Machine Status Register (MSR)
        public uint MSR { get; set; }

        // Segment Registers (SR0 - SR15)
        // I think these are related to BATS?
        public uint[] SR { get; private set; } = new uint[16];

        public void Reset()
        {
            Array.Fill<uint>(GPR, 0x0000_0000);
            Array.Fill<uint>(SPR, 0x0000_0000);
            Array.Fill<uint>(SR, 0x0000_0000);
            PC = 0x0000_0000;
            CR0 = 0x0000_0000;
            // TODO: initialize MSR to a sane value.
            MSR = 0x0000_0000;
        }
    }
}
