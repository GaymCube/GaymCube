using System;

namespace GaymCube.CPU
{
    class GekkoState
    {
        public enum Condition : UInt32
        {
            LT = 0x8000_0000,
            GT = 0x4000_0000,
            EQ = 0x2000_0000,
            SO = 0x1000_0000
        }

        // General Purpose Registers R0 - R31
        public UInt32[] GPR { get; } = new UInt32[32];

        // Special Purpose Registers SPR0 - SPR1023
        public UInt32[] SPR { get; } = new UInt32[1024];
        public UInt32 LR
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
        public UInt32 SRR0
        {
            get
            {
                return SPR[26];
            }
        }
        public UInt32 SRR1
        {
            get
            {
                return SPR[27];
            }
        }

        // Program Counter  (PC)
        public UInt32 PC { get; set; }

        // Condition Register 0 (CR0)
        // Bit 0: less than zero (sign-flag)
        // Bit 1: greater than zero
        // Bit 2: equal to zero (zero-flag)
        // Bit 3: summary overflow (copy of overflow flag in XER)
        public UInt32 CR0 { get; set; }

        // Machine Status Register (MSR)
        public UInt32 MSR { get; set; }

        public void Reset()
        {
            Array.Fill<UInt32>(GPR, 0x0000_0000);
            Array.Fill<UInt32>(SPR, 0x0000_0000);
            PC = 0x0000_0000;
            CR0 = 0x0000_0000;
            // TODO: initialize MSR to a sane value.
            MSR = 0x0000_0000;
        }
    }
}
