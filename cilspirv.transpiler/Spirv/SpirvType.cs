﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler;

namespace cilspirv.Spirv
{
    internal abstract record SpirvType : IInstructionGeneratable
    {
        public abstract SpirvTypeKind Kind { get; }

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context, out ID? resultId)
        {
            resultId = context.CreateID();
            return TypeInstructions(context, resultId.Value);
        }

        protected abstract IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId);
    }

    internal abstract record SpirvOpaqueType : SpirvType { }
    internal abstract record SpirvScalarType : SpirvType { }
    internal abstract record SpirvNumericType : SpirvScalarType { }
    internal abstract record SpirvCompositeType : SpirvType { }
    internal abstract record SpirvAggregateType : SpirvCompositeType { }

    internal sealed record SpirvVoidType : SpirvType
    {
        public override SpirvTypeKind Kind => SpirvTypeKind.Void;
        public override string ToString() => "void";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
        {
            yield return new OpTypeVoid() { Result = resultId };
        }
    }

    internal sealed record SpirvBooleanType : SpirvScalarType
    {
        public override SpirvTypeKind Kind => SpirvTypeKind.Boolean;
        public override string ToString() => "bool";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
        {
            yield return new OpTypeBool() { Result = resultId };
        }
    }

    internal sealed record SpirvIntegerType : SpirvNumericType
    {
        public int Width { get; init; }
        public bool IsSigned { get; init; }

        public override SpirvTypeKind Kind => SpirvTypeKind.Integer;
        public override string ToString() => $"{(IsSigned ? "u" : "")}int{Width}";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
        {
            yield return new OpTypeInt()
            {
                Result = resultId,
                Width = Width,
                Signedness = IsSigned ? 1 : 0
            };
        }
    }

    internal sealed record SpirvFloatingType : SpirvNumericType
    {
        public int Width { get; init; }

        public override SpirvTypeKind Kind => SpirvTypeKind.Integer;
        public override string ToString() => $"float{Width}";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
        {
            yield return new OpTypeFloat()
            {
                Result = resultId,
                Width = Width
            };
        }
    }

    internal sealed record SpirvVectorType : SpirvCompositeType
    {
        public SpirvScalarType? ComponentType { get; init; }
        public int ComponentCount { get; init; }

        public override SpirvTypeKind Kind => SpirvTypeKind.Vector;
        public override string ToString() => $"{ComponentType}Vec{ComponentCount}";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
        {
            yield return new OpTypeVector()
            {
                Result = resultId,
                ComponentType = context.IDOf(ComponentType ?? throw new InvalidOperationException("ComponentType is not set")),
                ComponentCount = ComponentCount
            };
        }
    }

    internal sealed record SpirvMatrixType : SpirvCompositeType
    {
        public SpirvVectorType? ColumnType { get; init; }
        public int ColumnCount { get; init; }
        public int RowCount => ColumnType?.ComponentCount ?? 0;
        public SpirvScalarType? ComponentType => ColumnType?.ComponentType;

        public override SpirvTypeKind Kind => SpirvTypeKind.Matrix;
        public override string ToString() => $"{ComponentType}Matrix{RowCount}x{ColumnCount}";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
        {
            yield return new OpTypeMatrix()
            {
                Result = resultId,
                ColumnType = context.IDOf(ColumnType ?? throw new InvalidOperationException("ColumnType is not set")),
                ColumnCount = ColumnCount
            };
        }
    }

    internal sealed record SpirvArrayType : SpirvAggregateType
    {
        public SpirvType? ElementType { get; init; }
        public int Length { get; init; }

        public override SpirvTypeKind Kind => SpirvTypeKind.Array;
        public override string ToString() => $"{ElementType}[{Length}]";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
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
                Result = resultId,
                ElementType = context.IDOf(ElementType ?? throw new InvalidOperationException("ElementType is not set")),
                Length = lengthId
            };
        }
    }

    internal sealed record SpirvStructType : SpirvAggregateType
    {
        public ImmutableArray<SpirvType> Members { get; init; }

        public override SpirvTypeKind Kind => SpirvTypeKind.Structure;
        public override string ToString() => $"{{{string.Join(", ", Members)}}}";
        protected override IEnumerator<Instruction> TypeInstructions(IInstructionGeneratorContext context, ID resultId)
        {
            yield return new OpTypeStruct()
            {
                Result = resultId,
                Members = Members
                    .Select(context.IDOf)
                    .ToImmutableArray()
            };
        }
    }
}
