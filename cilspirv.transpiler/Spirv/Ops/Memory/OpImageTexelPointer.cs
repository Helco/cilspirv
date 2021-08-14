// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpImageTexelPointer : MemoryInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Image { get; init; }
        public ID Coordinate { get; init; }
        public ID Sample { get; init; }

        public override OpCode OpCode => OpCode.OpImageTexelPointer;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Image, Coordinate, Sample };

        public OpImageTexelPointer() {}

        private OpImageTexelPointer(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Image = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Sample = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Image);
            codes[i++] = mapID(Coordinate);
            codes[i++] = mapID(Sample);
        }
    }
}

