using System;

namespace GaymCube.Util {
  static class BitUtil {
    public static UInt32 RotateLeft(UInt32 value, int amount) {
      if (amount != 0) {
        return (value << amount) | (value >> (32 - amount));
      }
      return value;
    }
  }
}
