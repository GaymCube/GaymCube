
namespace GaymCube.CPU
{
    partial class Gekko
    {
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
    }
}
