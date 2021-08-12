// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpVectorShuffle : CompositeInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Vector1 { get; init; }
        public ID Vector2 { get; init; }
        public ImmutableArray<LiteralNumber> Components { get; init; }

        public override OpCode OpCode => OpCode.OpVectorShuffle;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + Components.Length;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Vector1, Vector2 };

        public OpVectorShuffle() {}

        private OpVectorShuffle(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Vector1 = new ID(codes[i++]);
            Vector2 = new ID(codes[i++]);
            Components = codes.Skip(i).Take(end - i)
                .Select(x => (LiteralNumber)x)
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Vector1.Value;
            codes[i++] = Vector2.Value;
            foreach (var x in Components)
            {
                codes[i++] = x.Value;
            }
        }
    }
}

