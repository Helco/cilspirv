// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.SparseResidency })]
    public sealed record OpImageSparseRead : ImageInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Image { get; init; }
        public ID Coordinate { get; init; }
        public ImageOperands? ImageOperands { get; init; }

        public override OpCode OpCode => OpCode.OpImageSparseRead;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + (ImageOperands.HasValue ? 1 : 0);
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Image, Coordinate };

        public OpImageSparseRead() {}

        private OpImageSparseRead(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Image = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            if (i < end)
                ImageOperands = (ImageOperands)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = Image.Value;
            codes[i++] = Coordinate.Value;
            if (ImageOperands.HasValue)
            {
                codes[i++] = (uint)ImageOperands.Value;
            }
        }
    }
}

