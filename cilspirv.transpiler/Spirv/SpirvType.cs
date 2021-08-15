using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler;

namespace cilspirv.Spirv
{
    internal abstract record SpirvType : IInstructionGeneratable
    {
        public virtual IEnumerable<SpirvType> Dependencies => Enumerable.Empty<SpirvType>();
        public abstract IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context);
    }

    internal abstract record SpirvOpaqueType : SpirvType { }
    internal abstract record SpirvScalarType : SpirvType { }
    internal abstract record SpirvNumericType : SpirvScalarType { }
    internal abstract record SpirvCompositeType : SpirvType { }
    internal abstract record SpirvAggregateType : SpirvCompositeType { }

    internal sealed record SpirvVoidType : SpirvType
    {
        public override string ToString() => "Void";
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeVoid() { Result = context.CreateIDFor(this) };
        }
    }

    internal sealed record SpirvBooleanType : SpirvScalarType
    {
        public override string ToString() => "Bool";
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeBool() { Result = context.CreateIDFor(this) };
        }
    }

    internal sealed record SpirvIntegerType : SpirvNumericType
    {
        public int Width { get; init; }
        public bool IsSigned { get; init; }

        public override string ToString() => $"{(IsSigned ? "U" : "")}Int{Width}";
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeInt()
            {
                Result = context.CreateIDFor(this),
                Width = Width,
                Signedness = IsSigned ? 1 : 0
            };
        }
    }

    internal sealed record SpirvFloatingType : SpirvNumericType
    {
        public int Width { get; init; }

        public override string ToString() => $"Float{Width}";
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeFloat()
            {
                Result = context.CreateIDFor(this),
                Width = Width
            };
        }
    }

    internal sealed record SpirvVectorType : SpirvCompositeType
    {
        public SpirvScalarType? ComponentType { get; init; }
        public int ComponentCount { get; init; }

        public override string ToString() => $"{ComponentType}Vec{ComponentCount}";
        public override IEnumerable<SpirvType> Dependencies => new[] { ComponentType! }; // scalar types have no further dependencies
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeVector()
            {
                Result = context.CreateIDFor(this),
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

        public override string ToString() => $"{ComponentType}Matrix{RowCount}x{ColumnCount}";
        public override IEnumerable<SpirvType> Dependencies => new SpirvType[] { ColumnType!, ComponentType! };
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeMatrix()
            {
                Result = context.CreateIDFor(this),
                ColumnType = context.IDOf(ColumnType ?? throw new InvalidOperationException("ColumnType is not set")),
                ColumnCount = ColumnCount
            };
        }
    }

    internal sealed record SpirvArrayType : SpirvAggregateType
    {
        public SpirvType? ElementType { get; init; }
        public int Length { get; init; }

        public override string ToString() => $"{ElementType}[{Length}]";
        public override IEnumerable<SpirvType> Dependencies => new[] { ElementType! }.Concat(ElementType!.Dependencies);
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
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

    internal sealed record SpirvRuntimeArrayType : SpirvAggregateType
    {
        public SpirvType? ElementType { get; init; }

        public override string ToString() => $"{ElementType}[]";
        public override IEnumerable<SpirvType> Dependencies => new[] { ElementType! }.Concat(ElementType!.Dependencies);
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeRuntimeArray()
            {
                Result = context.CreateIDFor(this),
                ElementType = context.IDOf(ElementType ?? throw new InvalidOperationException("ElementType is not set"))
            };
        }
    }

    internal sealed record SpirvStructType : SpirvAggregateType
    {
        public ImmutableArray<SpirvType> Members { get; init; }

        public override string ToString() => $"{{{string.Join(", ", Members)}}}";
        public override IEnumerable<SpirvType> Dependencies => Members.Concat(Members.SelectMany(m => m.Dependencies));
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeStruct()
            {
                Result = context.CreateIDFor(this),
                Members = Members
                    .Select(context.IDOf)
                    .ToImmutableArray()
            };
        }
    }

    internal sealed record SpirvImageType : SpirvOpaqueType
    {
        public SpirvNumericType? SampledType { get; init; }
        public Dim Dim { get; init; }
        public bool? IsDepth { get; init; }
        public bool IsArray { get; init; }
        public bool IsMultisampled { get; init; }
        public bool? HasSampler { get; init; }
        public ImageFormat Format { get; init; }
        public AccessQualifier? Access { get; init; }

        public override string ToString()
        {
            var tags = Enumerable.Empty<string>();
            if (IsMultisampled == true)
                tags = tags.Append("MS");
            if (HasSampler == true)
                tags = tags.Append("Sampled");
            else if (HasSampler == false)
                tags = tags.Append("Storage");

            return Access switch
            {
                AccessQualifier.ReadOnly => "RO",
                AccessQualifier.WriteOnly => "WO",
                AccessQualifier.ReadWrite => "RW",
                _ => ""
            } + (IsArray ? "Array" : "") + (IsDepth == true ? "Depth" : "")
            + $"Image{Dim.ToString().Trim('D', 'i', 'm')}<{SampledType}>({Format}"
            + (tags.Any() ? $",{string.Join(',', tags)})" : ")");
        }

        public override IEnumerable<SpirvType> Dependencies => new[] { SampledType! };

        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeImage()
            {
                Result = context.CreateIDFor(this),
                SampledType = context.IDOf(SampledType ?? throw new InvalidOperationException("SampledType is not set")),
                Dim = Dim,
                Depth = IsDepth switch
                {
                    false => 0,
                    true => 1,
                    _ => 2
                },
                Arrayed = IsArray ? 1 : 0,
                MS = IsMultisampled ? 1 : 0,
                Sampled = HasSampler switch
                {
                    true => 1,
                    false => 2,
                    _ => 0
                },
                ImageFormat = Format,
                AccessQualifier = Access
            };
        }
    }

    internal sealed record SpirvSamplerType : SpirvOpaqueType
    {
        public override string ToString() => "Sampler";
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeSampler()
            {
                Result = context.CreateIDFor(this)
            };
        }
    }

    internal sealed record SpirvSampledImageType : SpirvOpaqueType
    {
        public SpirvImageType? ImageType { get; init; }
        public override string ToString() => "Sampled" + ImageType?.ToString();
        public override IEnumerable<SpirvType> Dependencies => new[] { ImageType! }.Concat(ImageType!.Dependencies);
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeSampledImage()
            {
                Result = context.CreateIDFor(this),
                ImageType = context.IDOf(ImageType ?? throw new InvalidOperationException("ImageType is not set"))
            };
        }
    }

    internal sealed record SpirvPointerType : SpirvType
    {
        public SpirvType? Type { get; init; }
        public StorageClass StorageClass { get; init; }
        public override string ToString() => $"Ptr<{Type}>({StorageClass})";
        public override IEnumerable<SpirvType> Dependencies => new[] { Type! }.Concat(Type!.Dependencies);
        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypePointer()
            {
                Result = context.CreateIDFor(this),
                Type = context.IDOf(Type ?? throw new InvalidOperationException("Type is not set")),
                StorageClass = StorageClass
            };
        }
    }

    internal sealed record SpirvFunctionType : SpirvType
    {
        public SpirvType? ReturnType { get; init; }
        public ImmutableArray<SpirvType> ParameterTypes { get; init; }
        public override string ToString() => $"{ReturnType}({string.Join(", ", ParameterTypes)})";

        public override IEnumerable<SpirvType> Dependencies =>
            new[] { ReturnType! }
            .Concat(ReturnType!.Dependencies)
            .Concat(ParameterTypes)
            .Concat(ParameterTypes.SelectMany(p => p.Dependencies));

        public override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeFunction()
            {
                Result = context.CreateIDFor(this),
                ReturnType = context.IDOf(ReturnType ?? throw new InvalidOperationException("ReturnType is not set")),
                Parameters = ParameterTypes.Select(context.IDOf).ToImmutableArray()
            };
        }
    }
}
