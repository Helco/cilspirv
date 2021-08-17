using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    public record SpirvImageType : SpirvOpaqueType
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

        internal override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
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

    public record SpirvSamplerType : SpirvOpaqueType
    {
        public override string ToString() => "Sampler";
        internal override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeSampler()
            {
                Result = context.CreateIDFor(this)
            };
        }
    }

    public record SpirvSampledImageType : SpirvOpaqueType
    {
        public SpirvImageType? ImageType { get; init; }
        public override string ToString() => "Sampled" + ImageType?.ToString();
        public override IEnumerable<SpirvType> Dependencies => new[] { ImageType! }.Concat(ImageType!.Dependencies);
        internal override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeSampledImage()
            {
                Result = context.CreateIDFor(this),
                ImageType = context.IDOf(ImageType ?? throw new InvalidOperationException("ImageType is not set"))
            };
        }
    }
}
