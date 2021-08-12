// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SubgroupShuffleINTEL })]
    public sealed record OpSubgroupShuffleDownINTEL : GroupInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Current { get; init; }
        public ID Next { get; init; }
        public ID Delta { get; init; }

        public override OpCode OpCode => OpCode.OpSubgroupShuffleDownINTEL;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Current, Next, Delta };

        public OpSubgroupShuffleDownINTEL() {}

        private OpSubgroupShuffleDownINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Current = new ID(codes[i++]);
            Next = new ID(codes[i++]);
            Delta = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = Current.Value;
            codes[i++] = Next.Value;
            codes[i++] = Delta.Value;
        }
    }
}

