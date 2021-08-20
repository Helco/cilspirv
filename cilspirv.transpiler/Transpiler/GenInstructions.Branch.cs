using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;

namespace cilspirv.Transpiler
{
    partial class Transpiler
    {
        partial class GenInstructions
        {
            private class MethodCallContext : ITranspilerMethodContext
            {
                private readonly GenInstructions gen;
                private ID? resultID;

                public TranspilerLibrary Library => gen.Library;
                public TranspilerModule Module => gen.Module;
                public TranspilerFunction Function => gen.Function;
                public TranspilerOptions Options => gen.Options;
                public ID CreateID() => gen.context.CreateID();
                public ID CreateIDFor(IInstructionGeneratable generatable) => gen.context.CreateIDFor(generatable);
                public ID IDOf(IInstructionGeneratable generatable) => gen.context.IDOf(generatable);
                public IEnumerable<T> OfType<T>() where T : IInstructionGeneratable => gen.context.OfType<T>();

                public (ID, SpirvType)? This { get; init; }
                public IReadOnlyList<(ID, SpirvType)> Parameters { get; init; } = Array.Empty<(ID, SpirvType)>();

                public ID? ResultID => resultID;
                ID ITranspilerMethodContext.ResultID
                {
                    get => resultID ?? (resultID = CreateID()).Value;
                    set
                    {
                        if (resultID != null)
                            throw new InvalidOperationException("Result ID was already generated or set");
                        resultID = value;
                    }
                }

                public MethodCallContext(GenInstructions gen) => this.gen = gen;
            }

            private void Call(MethodReference methodRef, bool isCtor)
            {
                // TODO: Coercing
                var callMethod = Library.MapMethod(methodRef);
                var paramCount = methodRef.Parameters.Count;
                var stackCount = paramCount + (!isCtor && methodRef.HasThis ? 1 : 0);

                if (Stack.Count < stackCount)
                    throw new InvalidOperationException($"Stack does not have all parameters");
                var thiz = Stack[Stack.Count - stackCount] as ValueStackEntry; // calling a method on a VarGroup still has a null this pointer
                var context = new MethodCallContext(this)
                {
                    This = stackCount > paramCount && Stack[Stack.Count - stackCount] is ValueStackEntry thisValue
                        ? (thisValue.ID, thisValue.Type)
                        : null,
                    Parameters = Stack
                        .TakeLast(paramCount)
                        .Select((e, i) => e as ValueStackEntry ?? throw new InvalidOperationException($"Parameter {i} is not a value entry"))
                        .Select(e => (e.ID, e.Type))
                        .ToArray()
                };
                Block.Instructions.AddRange(callMethod(context));

                Stack.RemoveRange(Stack.Count - stackCount, stackCount);
                if (context.ResultID != null)
                {
                    var returnType = (SpirvType)Library.MapType(isCtor
                        ? methodRef.DeclaringType
                        : methodRef.ReturnType);
                    Stack.Add(new ValueStackEntry(methodRef, context.ResultID.Value, returnType));
                }
            }

            private void Return()
            {
                // TODO: Check return type and coerce
                if (Function.ReturnType is SpirvVoidType)
                    Add(new OpReturn());
                else
                {
                    if (Pop() is not ValueStackEntry returnValue)
                        throw new InvalidOperationException("Top of stack is not SPIRV entry");
                    Add(new OpReturnValue()
                    {
                        Value = returnValue.ID
                    });
                }
                LeaveBlock();
            }

            private void Branch(ILInstruction target)
            {
                if (!blocks.TryGetValue(target.Offset, out var targetBlock))
                    blocks[target.Offset] = targetBlock = new BlockInfo(Stack.ToList());
                Add(new OpBranch()
                {
                    TargetLabel = context.IDOf(targetBlock.block)
                });
                LeaveBlock();
            }

            private void ConditionalBranch(ID conditionID, ILInstruction me)
            {
                var trueInstr = (ILInstruction)me.Operand;
                if (!blocks.TryGetValue(trueInstr.Offset, out var trueBlock))
                    blocks[trueInstr.Offset] = trueBlock = new BlockInfo(Stack.ToList());
                if (!blocks.TryGetValue(me.Next.Offset, out var falseBlock))
                    blocks[me.Next.Offset] = falseBlock = new BlockInfo(Stack.ToList());

                Add(new OpBranchConditional()
                {
                    Condition = conditionID,
                    TrueLabel = context.IDOf(trueBlock.block),
                    FalseLabel = context.IDOf(falseBlock.block)
                });
            }

            private void BranchEqual(ILInstruction me) => ConditionalBranch(CompareEqual(), me);
            private void BranchNotEqual(ILInstruction me) => ConditionalBranch(CompareNotEqual(), me);
            private void BranchGreater(ILInstruction me) => ConditionalBranch(CompareGreater(), me);
            private void BranchGreaterUnordered(ILInstruction me) => ConditionalBranch(CompareGreaterUnordered(), me);
            private void BranchGreaterOrEqual(ILInstruction me) => ConditionalBranch(CompareGreaterOrEqual(), me);
            private void BranchGreaterOrEqualUnordered(ILInstruction me) => ConditionalBranch(CompareGreaterOrEqualUnordered(), me);
            private void BranchLess(ILInstruction me) => ConditionalBranch(CompareLess(), me);
            private void BranchLessUnordered(ILInstruction me) => ConditionalBranch(CompareLessUnordered(), me);
            private void BranchLessOrEqual(ILInstruction me) => ConditionalBranch(CompareLessOrEqual(), me);
            private void BranchLessOrEqualUnordered(ILInstruction me) => ConditionalBranch(CompareLessOrEqualUnordered(), me);
            private void BranchFalsy(ILInstruction me) => ConditionalBranch(CompareFalsy(), me);
            private void BranchTruthy(ILInstruction me) => ConditionalBranch(CompareTruthy(), me);
        }
    }
}
