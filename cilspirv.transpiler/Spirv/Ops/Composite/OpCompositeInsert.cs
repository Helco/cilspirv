// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpCompositeInsert : CompositeInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Object { get; init; }
        public ID Composite { get; init; }
        public ImmutableArray<LiteralNumber> Indexes { get; init; }

        public override OpCode OpCode => OpCode.OpCompositeInsert;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + Indexes.Length;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Object, Composite };

        public OpCompositeInsert() {}

        private OpCompositeInsert(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Object = new ID(codes[i++]);
            Composite = new ID(codes[i++]);
            Indexes = codes.Skip(i).Take(end - i)
                .Select(x => (LiteralNumber)x)
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = Object.Value;
            codes[i++] = Composite.Value;
            foreach (var x in Indexes)
            {
                codes[i++] = x.Value;
            }
        }
    }
}

