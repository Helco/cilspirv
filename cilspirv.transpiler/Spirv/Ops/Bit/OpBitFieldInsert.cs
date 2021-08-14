// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Shader, Capability.BitInstructions })]
    public sealed record OpBitFieldInsert : BitInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Base { get; init; }
        public ID Insert { get; init; }
        public ID Offset { get; init; }
        public ID Count { get; init; }

        public override OpCode OpCode => OpCode.OpBitFieldInsert;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Base, Insert, Offset, Count };

        public OpBitFieldInsert() {}

        private OpBitFieldInsert(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Base = new ID(codes[i++]);
            Insert = new ID(codes[i++]);
            Offset = new ID(codes[i++]);
            Count = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Base);
            codes[i++] = mapID(Insert);
            codes[i++] = mapID(Offset);
            codes[i++] = mapID(Count);
        }
    }
}

