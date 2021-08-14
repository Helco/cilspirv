// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.3", Capabilities = new[] { Capability.GroupNonUniformShuffleRelative })]
    public sealed record OpGroupNonUniformShuffleDown : NonUniformInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Execution { get; init; }
        public ID Value { get; init; }
        public ID Delta { get; init; }

        public override OpCode OpCode => OpCode.OpGroupNonUniformShuffleDown;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Execution, Value, Delta };

        public OpGroupNonUniformShuffleDown() {}

        private OpGroupNonUniformShuffleDown(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Execution = new ID(codes[i++]);
            Value = new ID(codes[i++]);
            Delta = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Execution);
            codes[i++] = mapID(Value);
            codes[i++] = mapID(Delta);
        }
    }
}

