using System;

namespace GaymCube.CPU {
  interface IMemory {
    Byte ReadByte(UInt32 address);
    UInt16 ReadHalf(UInt32 address);
    UInt32 ReadWord(UInt32 address);
    void WriteByte(UInt32 address, Byte value);
    void WriteHalf(UInt32 address, UInt16 value);
    void WriteWord(UInt32 address, UInt32 value);
  }
}
