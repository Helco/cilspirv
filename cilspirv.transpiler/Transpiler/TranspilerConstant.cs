using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal abstract record TranspilerConstant : IDebugInstructionGeneratable
    {
        public string? Name { get; init; }
        public SpirvType Type { get; }

        protected TranspilerConstant(SpirvType type) => Type = type;

        public abstract IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context);

        public IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context)
        {
            if (Name != null)
                yield return new OpName()
                {
                    Target = context.IDOf(this),
                    Name = Name
                };
        }
    }

    internal sealed record TranspilerNumericConstant : TranspilerConstant
    {
        public ImmutableArray<LiteralNumber> Value { get; }

        public TranspilerNumericConstant(ImmutableArray<LiteralNumber> value, SpirvNumericType type) : base(type) => Value = value;

        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpConstant()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(Type),
                Value = Value
            };
        }
    }

    internal sealed record TranspilerBooleanConstant : TranspilerConstant
    {
        public bool Value { get; }

        public TranspilerBooleanConstant(bool value) : base(new SpirvBooleanType()) => Value = value;

        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
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
