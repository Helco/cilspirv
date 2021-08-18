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
                this.stack = initialStack ?? new List<StackEntry>();
        }

        private partial class GenInstructions : ITranspilerMethodContext
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
                        case (Code.Ret): Return(); break;

                        case (Code.Ldarga): // yes, currently no difference ldarga and larg
                        case (Code.Ldarg):
                        case (Code.Ldarga_S):
                        case (Code.Ldarg_S): LoadArgument(((ParameterReference)ilInstr.Operand).Index); break;
                        case (Code.Ldarg_0): LoadArgument(ILBody.Method.HasThis ? -1 : 0); break;
                        case (Code.Ldarg_1): LoadArgument(ILBody.Method.HasThis ? 0 : 1); break;
                        case (Code.Ldarg_2): LoadArgument(ILBody.Method.HasThis ? 1 : 2); break;
                        case (Code.Ldarg_3): LoadArgument(ILBody.Method.HasThis ? 2 : 3); break;

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

                        case (Code.Newobj): Call((MethodReference)ilInstr.Operand, isCtor: true); break;
                        case (Code.Call): Call((MethodReference)ilInstr.Operand, isCtor: false); break;

                        case (Code.Leave):
                        case (Code.Leave_S):
                        case (Code.Br):
                        case (Code.Br_S): Branch((ILInstruction)ilInstr.Operand); break;

                        default: throw new NotSupportedException($"Unsupported opcode {ilInstr.OpCode}");
                    }
                }

                Function.Blocks.Clear();
                foreach (var kv in blocks.OrderBy(kv => kv.Key))
                    Function.Blocks.Add(kv.Value.block);
            }

            private void LeaveBlock() => _currentBlockInfo = null;

            private void Add(SpirvInstruction instr) => Block.Instructions.Add(instr);

            ID IInstructionGeneratorContext.CreateID() => context.CreateID();
            ID IInstructionGeneratorContext.CreateIDFor(IInstructionGeneratable generatable) => context.CreateIDFor(generatable);
            ID IInstructionGeneratorContext.IDOf(IInstructionGeneratable generatable) => context.IDOf(generatable);
            IEnumerable<T> IInstructionGeneratorContext.OfType<T>() => context.OfType<T>();
        }
    }
}
