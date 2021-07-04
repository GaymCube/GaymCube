using System;

namespace GaymCube.CPU
{
    partial class Gekko
    {
        class UnhandledOpcodeException : Exception
        {
            public UnhandledOpcodeException(uint opcode)
                : base(string.Format("Unhandled opcode 0x{0:X8}", opcode))
            {
            }
        }

        public GekkoState State { get; private set; } = new GekkoState();
        public IMemory Memory { get; private set; }

        public Gekko(IMemory memory)
        {
            Memory = memory;
        }

        public void Reset()
        {
            State.Reset();
        }

        public void Run(int instructions)
        {
            while (instructions-- > 0)
            {
                uint opcode = Memory.ReadWord(State.PC);
                uint primary_op = opcode >> 26;
                uint secondary_op = opcode & 0x7FF;

                Console.WriteLine("PC=0x{0:X8} opcode=0x{1:X8}", State.PC, opcode);

                switch (primary_op)
                {
                    case 4:
                        // TODO: the decoding seems to be more complicated in reality.
                        switch (secondary_op)
                        {
                            case 144:
                            case 145:
                                PairedSingleMoveRegister(opcode);
                                break;
                        }
                        break;
                    case 14:
                        ADDI(opcode);
                        break;
                    case 15:
                        ADDIS(opcode);
                        break;
                    case 16:
                        BranchConditional(opcode);
                        break;
                    case 18:
                        Branch(opcode);
                        break;
                    case 19:
                        switch (secondary_op)
                        {
                            case 32:
                                BranchConditionalToLR(opcode);
                                break;
                            case 100:
                                ReturnFromInterrupt();
                                break;
                            case 300:
                                InstructionSync();
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
                        switch (secondary_op)
                        {
                            case 166:
                                MoveFromMSR(opcode);
                                break;
                            case 292:
                                MoveToMSR(opcode);
                                break;
                            case 420:
                                MoveToSR(opcode);
                                break;
                            case 678:
                                MoveFromSPR(opcode);
                                break;
                            case 934:
                                MoveToSPR(opcode);
                                break;
                            case 1196:
                                Sync();
                                break;
                            default:
                                throw new UnhandledOpcodeException(opcode);
                        }
                        break;
                    case 32:
                        LoadWordAndZero(opcode);
                        break;
                    case 36:
                        StoreWord(opcode);
                        break;
                    case 37:
                        StoreWordUpdate(opcode);
                        break;
                    case 56:
                        PairedSingleQuantizedLoad(opcode);
                        break;
                    default:
                        throw new UnhandledOpcodeException(opcode);
                }
            }
        }

        private void UpdateCR0(uint value)
        {
            // TODO: copy overflow bit from XER register into SO.
            State.CR0 = 0x0000_0000;

            if ((int)value < 0)
            {
                State.CR0 |= (uint)GekkoState.Condition.LT;
            }

            if ((int)value > 0)
            {
                State.CR0 |= (uint)GekkoState.Condition.GT;
            }

            if (value == 0)
            {
                State.CR0 |= (uint)GekkoState.Condition.EQ;
            }
        }

        private bool EvaluateBranchCondition(uint opcode)
        {
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

            return counterOK && conditionOK;
        }

        private uint LoadQuantized(ref uint address, int loadScale, int loadType)
        {
            float result = (float)(1 << loadScale);

            // See 2.1.2.11 Graphics Quantization Registers (GQRs) in PowerPC 750CL manual.
            switch (loadType)
            {
                case 0:
                    result *= BitConverter.Int32BitsToSingle((int)Memory.ReadWord(address));
                    address += sizeof(float);
                    break;
                case 4:
                    result *= Memory.ReadByte(address);
                    address += sizeof(byte);
                    break;
                case 5:
                    result *= Memory.ReadHalf(address);
                    address += sizeof(ushort);
                    break;
                case 6:
                    result *= (sbyte)Memory.ReadByte(address);
                    address += sizeof(sbyte);
                    break;
                case 7:
                    result *= (short)Memory.ReadHalf(address);
                    address += sizeof(short);
                    break;
                default:
                    throw new Exception(string.Format("Reserved load type: {}", loadType));
            }

            return (uint)BitConverter.SingleToInt32Bits(result);
        }
    }
}
