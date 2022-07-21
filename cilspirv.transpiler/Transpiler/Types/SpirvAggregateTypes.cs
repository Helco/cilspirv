using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    public record SpirvVectorType : SpirvCompositeType
    {
        public SpirvScalarType? ComponentType { get; init; }
        public int ComponentCount { get; init; }

        public override string ToString() => $"{ComponentType}Vec{ComponentCount}";
        public override IEnumerable<SpirvType> Dependencies => new[] { ComponentType! }; // scalar types have no further dependencies
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeVector()
            {
                Result = context.CreateIDFor(this),
                ComponentType = context.IDOf(ComponentType ?? throw new InvalidOperationException("ComponentType is not set")),
                ComponentCount = ComponentCount
            };
        }
    }

    public record SpirvMatrixType : SpirvCompositeType
    {
        public SpirvVectorType? ColumnType { get; init; }
        public int ColumnCount { get; init; }
        public int RowCount => ColumnType?.ComponentCount ?? 0;
        public SpirvScalarType? ComponentType => ColumnType?.ComponentType;

        public override string ToString() => $"{ComponentType}Matrix{RowCount}x{ColumnCount}";
        public override IEnumerable<SpirvType> Dependencies => new SpirvType[] { ColumnType!, ComponentType! };
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeMatrix()
            {
                Result = context.CreateIDFor(this),
                ColumnType = context.IDOf(ColumnType ?? throw new InvalidOperationException("ColumnType is not set")),
                ColumnCount = ColumnCount
            };
        }
    }

    public record SpirvArrayType : SpirvAggregateType
    {
        public SpirvType? ElementType { get; init; }
        public int Length { get; init; }

        public override string ToString() => $"{ElementType}[{Length}]";
        public override IEnumerable<SpirvType> Dependencies => new[] { ElementType! }.Concat(ElementType!.Dependencies);
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            var lengthId = context.CreateID();
            yield return new OpConstant()
            {
                Result = lengthId,
                ResultType = context.IDOf(new SpirvIntegerType()
                {
                    IsSigned = false,
                    Width = 32
                }),
                Value = LiteralNumber.ArrayFor(Length)
            };
            yield return new OpTypeArray()
            {
                Result = context.CreateIDFor(this),
                ElementType = context.IDOf(ElementType ?? throw new InvalidOperationException("ElementType is not set")),
                Length = lengthId
            };
        }
    }

    public record SpirvRuntimeArrayType : SpirvAggregateType
    {
        public SpirvType? ElementType { get; init; }

        public override string ToString() => $"{ElementType}[]";
        public override IEnumerable<SpirvType> Dependencies => new[] { ElementType! }.Concat(ElementType!.Dependencies);
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeRuntimeArray()
            {
                Result = context.CreateIDFor(this),
                ElementType = context.IDOf(ElementType ?? throw new InvalidOperationException("ElementType is not set"))
            };
        }
    }

    
}
