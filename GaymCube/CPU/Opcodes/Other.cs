using System;

namespace GaymCube.CPU {
  partial class Gekko {
    private void MoveFromMSR(UInt32 opcode) {
      UInt32 dst = (opcode >> 21) & 0x1F;
      State.GPR[dst] = State.MSR;
      State.PC += sizeof(UInt32);
    }

    private void MoveFromSPR(UInt32 opcode) {
      UInt32 spr_hi = (opcode >> 11) & 0x1F;
      UInt32 spr_lo = (opcode >> 16) & 0x1F;
      UInt32 src = (spr_hi << 5) | spr_lo;
      UInt32 dst = (opcode >> 21) & 0x1F;

      State.GPR[dst] = State.SPR[src];
      State.PC += sizeof(UInt32);
    }

    private void MoveToSPR(UInt32 opcode) {
      UInt32 spr_hi = (opcode >> 11) & 0x1F;
      UInt32 spr_lo = (opcode >> 16) & 0x1F;
      UInt32 dst = (spr_hi << 5) | spr_lo;
      UInt32 src = (opcode >> 21) & 0x1F;

      State.SPR[dst] = State.GPR[src];
      State.PC += sizeof(UInt32);
    }
  }
}
