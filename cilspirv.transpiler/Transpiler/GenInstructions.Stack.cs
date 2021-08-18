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
    partial class Transpiler
    {
        private record StackEntry
        {
            public StackEntry(object tag) => this.Tag = tag;
            protected StackEntry() => Tag = null;

            public object? Tag { get; }
        }

        private sealed record ValueStackEntry : StackEntry
        {
            public ID ID { get; }
            public SpirvType Type { get; }
            public StorageClass? StorageClass => (Type as SpirvPointerType)?.StorageClass;

            public ValueStackEntry(ID id, SpirvType type) : base() =>
                (ID, Type) = (id, type);

            public ValueStackEntry(object tag, ID id, SpirvType type) : base(tag) =>
                (ID, Type) = (id, type);
        }

        private sealed record ThisModule { }

        partial class GenInstructions
        {
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
