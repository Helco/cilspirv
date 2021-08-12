// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypeImage : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public ID SampledType { get; init; }
        public Dim Dim { get; init; }
        public LiteralNumber Depth { get; init; }
        public LiteralNumber Arrayed { get; init; }
        public LiteralNumber MS { get; init; }
        public LiteralNumber Sampled { get; init; }
        public ImageFormat ImageFormat { get; init; }
        public AccessQualifier? AccessQualifier { get; init; }

        public override OpCode OpCode => OpCode.OpTypeImage;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + (AccessQualifier.HasValue ? 1 : 0);
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result, SampledType };

        public OpTypeImage() {}

        private OpTypeImage(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            SampledType = new ID(codes[i++]);
            Dim = (Dim)codes[i++];
            Depth = (LiteralNumber)codes[i++];
            Arrayed = (LiteralNumber)codes[i++];
            MS = (LiteralNumber)codes[i++];
            Sampled = (LiteralNumber)codes[i++];
            ImageFormat = (ImageFormat)codes[i++];
            if (i < end)
                AccessQualifier = (AccessQualifier)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result.Value;
            codes[i++] = SampledType.Value;
            codes[i++] = (uint)Dim;
            codes[i++] = Depth.Value;
            codes[i++] = Arrayed.Value;
            codes[i++] = MS.Value;
            codes[i++] = Sampled.Value;
            codes[i++] = (uint)ImageFormat;
            if (AccessQualifier.HasValue)
            {
                codes[i++] = (uint)AccessQualifier.Value;
            }
        }
    }
}

