// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpControlBarrier : BarrierInstruction
    {
        public ID Execution { get; init; }
        public ID Memory { get; init; }
        public ID Semantics { get; init; }

        public override OpCode OpCode => OpCode.OpControlBarrier;
        public override int WordCount => 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Execution, Memory, Semantics };

        public OpControlBarrier() {}

        private OpControlBarrier(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Execution = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Semantics = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Execution);
            codes[i++] = mapID(Memory);
            codes[i++] = mapID(Semantics);
        }
    }
}

