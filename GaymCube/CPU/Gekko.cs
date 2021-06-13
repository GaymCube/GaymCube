using System;

namespace GaymCube.CPU {
  partial class Gekko {
    class UnhandledOpcodeException : Exception {
      public UnhandledOpcodeException(UInt32 opcode)
          : base(String.Format("Unhandled opcode 0x{0:X8}", opcode)) {
      }
    }

    public GekkoState State { get; private set; } = new GekkoState();
    public IMemory Memory { get; private set; }

    public Gekko(IMemory memory) {
      this.Memory = memory;
    }

    public void Reset() {
      State.Reset();
    }

    public void Run(int instructions) {
      while (instructions-- > 0) {
        UInt32 opcode = Memory.ReadWord(State.PC);
        UInt32 primary_op = opcode >> 26;
        UInt32 secondary_op = opcode & 0x7FF;

        Console.WriteLine("PC=0x{0:X8} opcode=0x{1:X8}", State.PC, opcode);

        switch (primary_op) {
          case 15:
            ADDIS(opcode);
            break;
          case 18:
            Branch(opcode);
            break;
          case 19:
            switch (secondary_op) {
              case 100:
                ReturnFromInterrupt();
                break;
              default:
                throw new UnhandledOpcodeException(opcode);
            }
            break;
          case 21:
            RLWINM(opcode);
            break;
          case 24:
            ORI(opcode);
            break;
          case 25:
            ORIS(opcode);
            break;
          case 31:
            switch (secondary_op) {
              case 166:
                MoveFromMSR(opcode);
                break;
              case 678:
                MoveFromSPR(opcode);
                break;
              case 934:
                MoveToSPR(opcode);
                break;
              default:
                throw new UnhandledOpcodeException(opcode);
            }
            break;
          default:
            throw new UnhandledOpcodeException(opcode);
        }
      }
    }

    private void UpdateCR0(UInt32 value) {
      // TODO: copy overflow bit from XER register into SO.
      State.CR0 = 0x0000_0000;

      if ((Int32)value < 0) {
        State.CR0 |= (UInt32)GekkoState.Condition.LT;
      }

      if ((Int32)value > 0) {
        State.CR0 |= (UInt32)GekkoState.Condition.GT;
      }

      if (value == 0) {
        State.CR0 |= (UInt32)GekkoState.Condition.EQ;
      }
    }
  }
}
