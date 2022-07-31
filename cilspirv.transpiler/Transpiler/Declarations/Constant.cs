using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler.Declarations
{
    internal abstract record Constant : IDebugInstructionGeneratable
    {
        public string? Name { get; init; }
        public SpirvType Type { get; }

        protected Constant(SpirvType type) => Type = type;

        public abstract IEnumerator<Instruction> GenerateInstructions(IIDMapper context);

        public IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context)
        {
            if (Name != null)
                yield return new OpName()
                {
                    Target = context.IDOf(this),
                    Name = Name
                };
        }
    }

    internal record NumericConstant : Constant
    {
        public ImmutableArray<LiteralNumber> Value { get; }

        public NumericConstant(ImmutableArray<LiteralNumber> value, SpirvNumericType type) : base(type) => Value = value;
        public NumericConstant(int value) : base(new SpirvIntegerType() { Width = 32, IsSigned = true }) => Value = LiteralNumber.ArrayFor(value);
        public NumericConstant(long value) : base(new SpirvIntegerType() { Width = 64, IsSigned = true }) => Value = LiteralNumber.ArrayFor(value);
        public NumericConstant(float value) : base(new SpirvFloatingType() { Width = 32 }) => Value = LiteralNumber.ArrayFor(value);
        public NumericConstant(double value) : base(new SpirvFloatingType() { Width = 64 }) => Value = LiteralNumber.ArrayFor(value);

        public virtual bool Equals(NumericConstant? other) =>
            base.Equals(other) &&
            Value.ValueEquals(other.Value);

        public override int GetHashCode() =>
            Value.Aggregate(base.GetHashCode(), HashCode.Combine);

        public override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpConstant()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(Type),
                Value = Value
            };
        }
    }

    internal sealed record BooleanConstant : Constant
    {
        public bool Value { get; }

        public BooleanConstant(bool value) : base(new SpirvBooleanType()) => Value = value;

        public override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return Value
                ? new OpConstantTrue()
                {
                    Result = context.CreateIDFor(this),
                    ResultType = context.IDOf(Type)
                }
                : new OpConstantFalse()
                {
                    Result = context.CreateIDFor(this),
                    ResultType = context.IDOf(Type)
                };
        }
    }
}
