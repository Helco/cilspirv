using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    public record SpirvBooleanType : SpirvScalarType
    {
        public override string ToString() => "Bool";
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeBool() { Result = context.CreateIDFor(this) };
        }
    }

    public record SpirvIntegerType : SpirvNumericType
    {
        public int Width { get; init; }
        public bool IsSigned { get; init; }

        public override string ToString() => $"{(IsSigned ? "U" : "")}Int{Width}";
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeInt()
            {
                Result = context.CreateIDFor(this),
                Width = Width,
                Signedness = IsSigned ? 1 : 0
            };
        }
    }

    public record SpirvFloatingType : SpirvNumericType
    {
        public int Width { get; init; }

        public override string ToString() => $"Float{Width}";
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeFloat()
            {
                Result = context.CreateIDFor(this),
                Width = Width
            };
        }
    }
}
