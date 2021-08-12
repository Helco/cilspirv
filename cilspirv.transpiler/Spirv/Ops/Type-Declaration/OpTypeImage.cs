// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypeImage : TypeDeclarationInstruction
    {
        public ID Result1 { get; init; }
        public ID SampledType { get; init; }
        public Dim Dim2 { get; init; }
        public LiteralNumber Depth { get; init; }
        public LiteralNumber Arrayed { get; init; }
        public LiteralNumber MS { get; init; }
        public LiteralNumber Sampled { get; init; }
        public ImageFormat ImageFormat3 { get; init; }
        public AccessQualifier? AccessQualifier4 { get; init; }

        public override OpCode OpCode => OpCode.OpTypeImage;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + (AccessQualifier4.HasValue ? 1 : 0);
        public override ID? ResultID => Result1;

        public override IEnumerable<ID> AllIDs => new[] { Result1, SampledType };

        public OpTypeImage() {}

        private OpTypeImage(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result1 = new ID(codes[i++]);
            SampledType = new ID(codes[i++]);
            Dim2 = (Dim)codes[i++];
            Depth = (LiteralNumber)codes[i++];
            Arrayed = (LiteralNumber)codes[i++];
            MS = (LiteralNumber)codes[i++];
            Sampled = (LiteralNumber)codes[i++];
            ImageFormat3 = (ImageFormat)codes[i++];
            if (i < end)
                AccessQualifier4 = (AccessQualifier)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result1.Value;
            codes[i++] = SampledType.Value;
            codes[i++] = (uint)Dim2;
            codes[i++] = Depth.Value;
            codes[i++] = Arrayed.Value;
            codes[i++] = MS.Value;
            codes[i++] = Sampled.Value;
            codes[i++] = (uint)ImageFormat3;
            if (AccessQualifier4.HasValue)
            {
                codes[i++] = (uint)AccessQualifier4.Value;
            }
        }
    }
}

