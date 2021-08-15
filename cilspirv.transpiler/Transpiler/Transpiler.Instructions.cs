using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Spirv;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;
using cilspirv.Spirv.Ops;
using System.Collections.Immutable;

namespace cilspirv.Transpiler
{
    partial class Transpiler : ITranspilerMethodContext
    {
        private record StackEntry
        {
            public ID ID { get; init; }
            public SpirvType Type { get; init; } = new SpirvVoidType();
        }

        private void TranspileInstructions(TranspilerDefinedFunction function, MethodBody ilBody)
        {
            var blocks = new Dictionary<int, (TranspilerBlock, List<StackEntry>)>();
            var curStack = new List<StackEntry>(ilBody.MaxStackSize);
            TranspilerBlock? curBlock = null;

            ILInstruction? prefix;
            foreach (var ilInstr in ilBody.Instructions)
            {
                if (curBlock == null)
                {
                    if (blocks.TryGetValue(ilInstr.Offset, out var premade))
                        (curBlock, curStack) = premade;
                    else
                    {
                        blocks[ilInstr.Offset] = (
                            curBlock = new TranspilerBlock(),
                            curStack = new List<StackEntry>());
                    }
                    function.Blocks.Add(curBlock);
                }

                if (ilInstr.OpCode.OpCodeType == OpCodeType.Prefix)
                {
                    prefix = ilInstr;
                    continue;
                }
                prefix = null;

                switch(ilInstr.OpCode.Code)
                {
                    case (Code.Nop): curBlock.Instructions.Add(new OpNop()); break;
                    case (Code.Pop): Pop(); break;
                    case (Code.Ret): Return(); break;

                    case (Code.Ldarg): PushArgument((ushort)ilInstr.Operand); break;
                    case (Code.Ldarg_S): PushArgument((byte)ilInstr.Operand); break;
                    case (Code.Ldarg_0): PushArgument(0); break;
                    case (Code.Ldarg_1): PushArgument(1); break;
                    case (Code.Ldarg_2): PushArgument(2); break;
                    case (Code.Ldarg_3): PushArgument(3); break;

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

                    case (Code.Newobj): Call((MethodReference)ilInstr.Operand, isCtor: true); break;
                    case (Code.Call): Call((MethodReference)ilInstr.Operand, isCtor: false); break;

                    case (Code.Leave):
                    case (Code.Leave_S):
                    case (Code.Br):
                    case (Code.Br_S): Branch((ILInstruction)ilInstr.Operand); break;

                    default: throw new NotSupportedException($"Unsupported opcode {ilInstr.OpCode}");
                }
            }

            SpirvType SpirvTypeOf<T>() => Library.MapType<T>().Type;
            ID TypeIdOf<T>() => generatorContext.IDOf(Library.MapType<T>());

            void PushID(ID id, SpirvType type) => curStack!.Add(new StackEntry()
            {
                ID = id,
                Type = type
            });

            void PushObj(IInstructionGeneratable generatable, SpirvType type) =>
                PushID(generatorContext.IDOf(generatable), type);

            void PushArgument(int argI) =>
                PushObj(function.Parameters[argI], function.Parameters[argI].Type);

            void PushConstant<T>(ImmutableArray<LiteralNumber> literal)
            {
                var spirvType = SpirvTypeOf<T>() as SpirvNumericType;
                if (spirvType is null)
                    throw new InvalidOperationException($"Instructions can only push numeric constants");

                var constant = new TranspilerNumericConstant(literal, spirvType);
                PushObj(constant, spirvType);
            }
            void PushI4(int value) => PushConstant<int>(LiteralNumber.ArrayFor(value));
            void PushI8(long value) => PushConstant<long>(LiteralNumber.ArrayFor(value));
            void PushR4(float value) => PushConstant<float>(LiteralNumber.ArrayFor(value));
            void PushR8(double value) => PushConstant<double>(LiteralNumber.ArrayFor(value));

            void Pop()
            {
                if (!curStack.Any())
                    throw new InvalidOperationException("Stack is empty, cannot pop");
                curStack.RemoveAt(curStack.Count - 1);
            }

            void LoadLocal(int variableI)
            {
                var id = generatorContext.CreateID();
                var variable = function.Variables[variableI];
                curBlock.Instructions.Add(new OpLoad()
                {
                    Result = id,
                    ResultType = generatorContext.IDOf(variable.ElementType),
                    Pointer = generatorContext.IDOf(variable)
                });
                PushID(id, variable.ElementType);
            }

            void PushLocalAddress(VariableReference variableRef)
            {
                var variable = function.Variables[variableRef.Index];
                PushObj(variable, variable.PointerType);
            }

            void StoreLocal(int variableI)
            {
                // TODO: Coercing
                var variable = function.Variables[variableI];
                if (curStack.LastOrDefault()?.Type != variable.ElementType)
                    throw new InvalidOperationException($"Cannot store {curStack.LastOrDefault()?.Type} in variable of type {variable.ElementType}");
                curBlock.Instructions.Add(new OpStore()
                {
                    Pointer = generatorContext.IDOf(variable),
                    Object = curStack.Last().ID
                });
                curStack.RemoveAt(curStack.Count - 1);
            }

            void Call(MethodReference methodRef, bool isCtor)
            {
                // TODO: Coercing
                var callMethod = Library.MapMethod(methodRef);
                var paramCount = methodRef.Parameters.Count;
                var stackCount = paramCount + (!isCtor && methodRef.HasThis ? 1 : 0);

                if (curStack.Count < stackCount)
                    throw new InvalidOperationException($"Stack does not have all parameters");
                var parameters = curStack.TakeLast(paramCount).Select(e => (e.ID, e.Type)).ToArray();
                curBlock.Instructions.AddRange(callMethod(this, parameters, out var resultId));
                if (stackCount > paramCount)
                    curBlock.Instructions.Add(new OpStore()
                    {
                        Pointer = curStack.ElementAt(curStack.Count - stackCount).ID,
                        Object = resultId ?? throw new NullReferenceException("Unexpected null return value")
                    });

                curStack.RemoveRange(curStack.Count - stackCount, stackCount);
                if (resultId != null)
                {
                    var returnType = isCtor
                        ? methodRef.DeclaringType
                        : methodRef.ReturnType;
                    PushID(resultId.Value, Library.MapType(returnType).Type);
                }
            }

            void Branch(ILInstruction target)
            {
                if (!blocks.TryGetValue(target.Offset, out var targetBlock))
                    blocks[target.Offset] = targetBlock = (new TranspilerBlock(), new List<StackEntry>());
                curBlock.Instructions.Add(new OpBranch()
                {
                    TargetLabel = generatorContext.IDOf(targetBlock.Item1)
                });
                curBlock = null;
            }

            void Return()
            {
                // TODO: Check return type and coerce
                if (function.ReturnType.Type is SpirvVoidType)
                    curBlock.Instructions.Add(new OpReturn());
                else
                    curBlock.Instructions.Add(new OpReturnValue()
                    {
                        Value = curStack.Last().ID
                    }); 
                curBlock = null;
            }
        }

        ID IInstructionGeneratorContext.CreateID() => generatorContext.CreateID();
        ID IInstructionGeneratorContext.CreateIDFor(IInstructionGeneratable generatable) => generatorContext.CreateIDFor(generatable);
        ID IInstructionGeneratorContext.IDOf(IInstructionGeneratable generatable) => generatorContext.IDOf(generatable);
        IEnumerable<T> IInstructionGeneratorContext.OfType<T>() => generatorContext.OfType<T>();
    }
}