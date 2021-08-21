using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;

namespace cilspirv.Transpiler
{
    partial class Transpiler
    {
        private class BlockInfo
        {
            public readonly TranspilerBlock block = new TranspilerBlock();
            public readonly List<StackEntry> stack;

            public BlockInfo(List<StackEntry>? initialStack = null) =>
                stack = initialStack ?? new List<StackEntry>();
        }

        private partial class GenInstructions
        {
            private readonly IInstructionGeneratorContext context;
            private readonly ID thisID;
            private readonly Dictionary<int, BlockInfo> blocks = new Dictionary<int, BlockInfo>();

            public TranspilerLibrary Library { get; }
            public TranspilerModule Module { get; }
            public TranspilerOptions Options { get; }
            public TranspilerDefinedFunction Function { get; }
            public MethodBody ILBody { get; }

            private ILInstruction? currentPrefix;
            private ILInstruction currentInstruction;

            private BlockInfo? _currentBlockInfo;
            private BlockInfo CurrentBlockInfo
            {
                get
                {
                    if (_currentBlockInfo != null)
                        return _currentBlockInfo;
                    if (blocks.TryGetValue(currentInstruction.Offset, out var blockInfo))
                        return _currentBlockInfo = blockInfo;

                    _currentBlockInfo = new BlockInfo();
                    blocks.Add(currentInstruction.Offset, _currentBlockInfo);
                    return _currentBlockInfo;
                }
            }
            private TranspilerBlock Block => CurrentBlockInfo.block;
            private List<StackEntry> Stack => CurrentBlockInfo.stack;

            public GenInstructions(Transpiler transpiler, TranspilerDefinedFunction function, MethodBody ilBody)
            {
                context = transpiler.generatorContext;
                Library = transpiler.Library;
                Module = transpiler.Module;
                Options = transpiler.Options;
                Function = function;
                ILBody = ilBody;
                thisID = context.IDOf(function);

                currentInstruction = ilBody.Instructions.First();
            }

            public void GenerateInstructions()
            {
                foreach (var ilInstr in ILBody.Instructions)
                {
                    currentInstruction = ilInstr;

                    if (blocks.TryGetValue(ilInstr.Offset, out var fallthroughBlock) && fallthroughBlock != CurrentBlockInfo)
                    {
                        Add(new OpBranch()
                        {
                            TargetLabel = context.IDOf(fallthroughBlock.block)
                        });
                        LeaveBlock();
                    }

                    if (ilInstr.OpCode.OpCodeType == OpCodeType.Prefix)
                    {
                        currentPrefix = ilInstr;
                        continue;
                    }
                    currentPrefix = null;

                    switch (ilInstr.OpCode.Code)
                    {
                        case (Code.Nop): Add(new OpNop()); break;
                        case (Code.Pop): Pop(); break;
                        case (Code.Dup): Dup(); break;
                        case (Code.Ret): Return(); break;
                        case (Code.Newobj): Call((MethodReference)ilInstr.Operand, isCtor: true); break;
                        case (Code.Call): Call((MethodReference)ilInstr.Operand, isCtor: false); break;

                        case (Code.Ldarga): // yes, currently no difference ldarga and larg
                        case (Code.Ldarg):
                        case (Code.Ldarga_S):
                        case (Code.Ldarg_S): LoadArgument(((ParameterReference)ilInstr.Operand).Index); break;
                        case (Code.Ldarg_0): LoadArgument(ILBody.Method.HasThis ? -1 : 0); break;
                        case (Code.Ldarg_1): LoadArgument(ILBody.Method.HasThis ? 0 : 1); break;
                        case (Code.Ldarg_2): LoadArgument(ILBody.Method.HasThis ? 1 : 2); break;
                        case (Code.Ldarg_3): LoadArgument(ILBody.Method.HasThis ? 2 : 3); break;

                        case (Code.Starg):
                        case (Code.Starg_S): StoreArgument(((ParameterReference)ilInstr.Operand).Index); break;

                        case (Code.Ldc_I4): PushI4((int)ilInstr.Operand); break;
                        case (Code.Ldc_I4_S): PushI4((sbyte)ilInstr.Operand); break;
                        case (Code.Ldc_I4_0): PushI4(0); break;
                        case (Code.Ldc_I4_1): PushI4(1); break;
                        case (Code.Ldc_I4_2): PushI4(2); break;
                        case (Code.Ldc_I4_3): PushI4(3); break;
                        case (Code.Ldc_I4_4): PushI4(4); break;
                        case (Code.Ldc_I4_5): PushI4(5); break;
                        case (Code.Ldc_I4_6): PushI4(6); break;
                        case (Code.Ldc_I4_7): PushI4(7); break;
                        case (Code.Ldc_I4_8): PushI4(8); break;
                        case (Code.Ldc_I4_M1): PushI4(-1); break;
                        case (Code.Ldc_I8): PushI8((long)ilInstr.Operand); break;
                        case (Code.Ldc_R4): PushR4((float)ilInstr.Operand); break;
                        case (Code.Ldc_R8): PushR8((double)ilInstr.Operand); break;

                        case (Code.Ldloc): LoadLocal((ushort)ilInstr.Operand); break;
                        case (Code.Ldloc_S): LoadLocal((byte)ilInstr.Operand); break;
                        case (Code.Ldloc_0): LoadLocal(0); break;
                        case (Code.Ldloc_1): LoadLocal(1); break;
                        case (Code.Ldloc_2): LoadLocal(2); break;
                        case (Code.Ldloc_3): LoadLocal(3); break;
                        case (Code.Ldloca): PushLocalAddress((VariableReference)ilInstr.Operand); break;
                        case (Code.Ldloca_S): PushLocalAddress((VariableReference)ilInstr.Operand); break;

                        case (Code.Stloc): StoreLocal((ushort)ilInstr.Operand); break;
                        case (Code.Stloc_S): StoreLocal((byte)ilInstr.Operand); break;
                        case (Code.Stloc_0): StoreLocal(0); break;
                        case (Code.Stloc_1): StoreLocal(1); break;
                        case (Code.Stloc_2): StoreLocal(2); break;
                        case (Code.Stloc_3): StoreLocal(3); break;

                        case (Code.Ldfld): LoadField((FieldReference)ilInstr.Operand); break;
                        case (Code.Ldflda): LoadFieldAddress((FieldReference)ilInstr.Operand); break;
                        case (Code.Stfld): StoreField((FieldReference)ilInstr.Operand); break;

                        case (Code.Ldobj): LoadObject(); break;
                        case (Code.Stobj): StoreObject(); break;

                        case (Code.Conv_I):
                        case (Code.Conv_Ovf_I):
                        case (Code.Conv_Ovf_I_Un): ConvertToSigned(Options.NativeIntWidth); break;
                        case (Code.Conv_I1):
                        case (Code.Conv_Ovf_I1):
                        case (Code.Conv_Ovf_I1_Un): ConvertToSigned(8); break;
                        case (Code.Conv_I2):
                        case (Code.Conv_Ovf_I2):
                        case (Code.Conv_Ovf_I2_Un): ConvertToSigned(16); break;
                        case (Code.Conv_I4):
                        case (Code.Conv_Ovf_I4):
                        case (Code.Conv_Ovf_I4_Un): ConvertToSigned(32); break;
                        case (Code.Conv_I8):
                        case (Code.Conv_Ovf_I8):
                        case (Code.Conv_Ovf_I8_Un): ConvertToSigned(64); break;
                        case (Code.Conv_U):
                        case (Code.Conv_Ovf_U):
                        case (Code.Conv_Ovf_U_Un): ConvertToUnsigned(Options.NativeIntWidth); break;
                        case (Code.Conv_U1):
                        case (Code.Conv_Ovf_U1):
                        case (Code.Conv_Ovf_U1_Un): ConvertToUnsigned(8); break;
                        case (Code.Conv_U2):
                        case (Code.Conv_Ovf_U2):
                        case (Code.Conv_Ovf_U2_Un): ConvertToUnsigned(16); break;
                        case (Code.Conv_U4):
                        case (Code.Conv_Ovf_U4):
                        case (Code.Conv_Ovf_U4_Un): ConvertToUnsigned(32); break;
                        case (Code.Conv_U8):
                        case (Code.Conv_Ovf_U8):
                        case (Code.Conv_Ovf_U8_Un): ConvertToUnsigned(64); break;
                        case (Code.Conv_R_Un): ConvertToFloating(64); break;
                        case (Code.Conv_R4): ConvertToFloating(32); break;
                        case (Code.Conv_R8): ConvertToFloating(64); break;

                        case (Code.Add):
                        case (Code.Add_Ovf):
                        case (Code.Add_Ovf_Un): Addition(); break;
                        case (Code.Sub):
                        case (Code.Sub_Ovf):
                        case (Code.Sub_Ovf_Un): Subtraction(); break;
                        case (Code.Mul):
                        case (Code.Mul_Ovf):
                        case (Code.Mul_Ovf_Un): Multiplication(); break;
                        case (Code.Div):
                        case (Code.Div_Un): Division(); break;
                        case (Code.Rem):
                        case (Code.Rem_Un): Remainder(); break;
                        case (Code.Neg): Negate(); break;

                        case (Code.And): BitwiseAnd(); break;
                        case (Code.Or): BitwiseOr(); break;
                        case (Code.Xor): BitwiseXor(); break;
                        case (Code.Not): BitwiseNot(); break;
                        case (Code.Shl): ShiftLeftLogical(); break;
                        case (Code.Shr): ShiftRightArithmetic(); break;
                        case (Code.Shr_Un): ShiftRightLogical(); break;

                        case (Code.Ceq): PushCompareEqual(); break;
                        case (Code.Cgt): PushCompareGreater(); break;
                        case (Code.Cgt_Un): PushCompareGreaterUnordered(); break;
                        case (Code.Clt): PushCompareLess(); break;
                        case (Code.Clt_Un): PushCompareLessUnordered(); break;

                        case (Code.Leave):
                        case (Code.Leave_S):
                        case (Code.Br):
                        case (Code.Br_S): Branch(ilInstr); break;
                        case (Code.Beq):
                        case (Code.Beq_S): BranchEqual(ilInstr); break;
                        case (Code.Bne_Un):
                        case (Code.Bne_Un_S): BranchNotEqual(ilInstr); break;
                        case (Code.Bgt):
                        case (Code.Bgt_S): BranchGreater(ilInstr); break;
                        case (Code.Bgt_Un):
                        case (Code.Bgt_Un_S): BranchGreaterUnordered(ilInstr); break;
                        case (Code.Bge):
                        case (Code.Bge_S): BranchGreaterOrEqual(ilInstr); break;
                        case (Code.Bge_Un):
                        case (Code.Bge_Un_S): BranchGreaterOrEqualUnordered(ilInstr); break;
                        case (Code.Blt):
                        case (Code.Blt_S): BranchLess(ilInstr); break;
                        case (Code.Blt_Un):
                        case (Code.Blt_Un_S): BranchLessUnordered(ilInstr); break;
                        case (Code.Ble):
                        case (Code.Ble_S): BranchLessOrEqual(ilInstr); break;
                        case (Code.Ble_Un):
                        case (Code.Ble_Un_S): BranchLessOrEqualUnordered(ilInstr); break;
                        case (Code.Brfalse):
                        case (Code.Brfalse_S): BranchFalsy(ilInstr); break;
                        case (Code.Brtrue):
                        case (Code.Brtrue_S): BranchTruthy(ilInstr); break;


                        default: throw new NotSupportedException($"Unsupported opcode {ilInstr.OpCode}");
                    }
                }

                Function.Blocks.Clear();
                foreach (var kv in blocks.OrderBy(kv => kv.Key))
                    Function.Blocks.Add(kv.Value.block);
            }

            private void LeaveBlock() => _currentBlockInfo = null;

            private void Add(SpirvInstruction instr) => Block.Instructions.Add(instr);
        }
    }
}
