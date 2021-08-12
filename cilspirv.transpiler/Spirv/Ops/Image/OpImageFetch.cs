// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpImageFetch : ImageInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Image { get; init; }
        public ID Coordinate { get; init; }
        public ImageOperands? ImageOperands3 { get; init; }

        public override OpCode OpCode => OpCode.OpImageFetch;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + (ImageOperands3.HasValue ? 1 : 0);
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Image, Coordinate };

        public OpImageFetch() {}

        private OpImageFetch(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Image = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            if (i < end)
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
            codes[i++] = Image.Value;
            codes[i++] = Coordinate.Value;
            if (ImageOperands3.HasValue)
            {
                codes[i++] = (uint)ImageOperands3.Value;
            }
        }
    }
}

