using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;

namespace cilspirv.Transpiler
{
    internal record StackEntry
    {
        public StackEntry(object tag) => this.Tag = tag;
        protected StackEntry() => Tag = null;

        public object? Tag { get; }
    }

    internal sealed record ValueStackEntry : StackEntry
    {
        public ID ID { get; }
        public SpirvType Type { get; }
        public StorageClass? StorageClass => (Type as SpirvPointerType)?.StorageClass;

        public ValueStackEntry(ID id, SpirvType type) : base() =>
            (ID, Type) = (id, type);

        public ValueStackEntry(object tag, ID id, SpirvType type) : base(tag) =>
            (ID, Type) = (id, type);
    }

    internal sealed record ThisModuleTag { }

    partial class Transpiler
    {
        partial class GenInstructions
        {
            private void SetupCurrentStack()
            {
                var previousBlocks = currentBlockInfo.PreviousBlocks;
                var length = previousBlocks.FirstOrDefault()?.Stack?.Count ?? 0;
                if (previousBlocks.Any(b => b.Stack.Count != length))
                    throw new InvalidOperationException("Some previous block brings a stack of another size");

                var stack = new List<StackEntry>(length);
                for (int i = 0; i < length; i++)
                {
                    var template = previousBlocks.First().Stack[i];
                    if (template is not ValueStackEntry templateValue)
                        throw new InvalidOperationException("Previous blocks can only bring values through the stack");

                    var newEntry = new ValueStackEntry(context.CreateID(), templateValue.Type);
                    Add(new OpPhi()
                    {
                        Result = newEntry.ID,
                        ResultType = context.IDOf(newEntry.Type),
                        Operands = previousBlocks
                            .Select(b => (((ValueStackEntry)b.Stack[i]).ID, context.IDOf(b.block)))
                            .ToImmutableArray()
                    });
                    stack.Add(newEntry);
                }
                currentBlockInfo.Stack = stack;
            }

            private void PushConstant(ImmutableArray<LiteralNumber> literal, SpirvNumericType spirvType)
            {
                var constant = new TranspilerNumericConstant(literal, spirvType);
                Stack.Add(new ValueStackEntry(context.IDOf(constant), spirvType));
            }
            private void PushI4(int value) => PushConstant(LiteralNumber.ArrayFor(value), new SpirvIntegerType() { Width = 32, IsSigned = true });
            private void PushI8(long value) => PushConstant(LiteralNumber.ArrayFor(value), new SpirvIntegerType() { Width = 64, IsSigned = true });
            private void PushR4(float value) => PushConstant(LiteralNumber.ArrayFor(value), new SpirvFloatingType() { Width = 32 });
            private void PushR8(double value) => PushConstant(LiteralNumber.ArrayFor(value), new SpirvFloatingType() { Width = 64 });

            private ValueStackEntry PopValue() =>
                Pop() as ValueStackEntry
                ?? throw new InvalidOperationException("Top of stack is not a value");

            private StackEntry Pop()
            {
                var result = Peek();
                Stack.RemoveAt(Stack.Count - 1);
                return result;
            }

            private StackEntry Peek() => Stack.Any()
                ? Stack.Last()
                : throw new InvalidOperationException("Stack is empty, cannot pop");

            private StackEntry Dup()
            {
                var result = Peek();
                Stack.Add(result);
                return result;
            }
        }
    }
}
