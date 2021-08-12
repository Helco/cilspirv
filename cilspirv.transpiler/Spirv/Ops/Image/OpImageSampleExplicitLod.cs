// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpImageSampleExplicitLod : ImageInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID SampledImage { get; init; }
        public ID Coordinate { get; init; }
        public ImageOperands ImageOperands3 { get; init; }

        public override OpCode OpCode => OpCode.OpImageSampleExplicitLod;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, SampledImage, Coordinate };

        public OpImageSampleExplicitLod() {}

        private OpImageSampleExplicitLod(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            SampledImage = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            ImageOperands3 = (ImageOperands)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = SampledImage.Value;
            codes[i++] = Coordinate.Value;
            codes[i++] = (uint)ImageOperands3;
        }
    }
}

