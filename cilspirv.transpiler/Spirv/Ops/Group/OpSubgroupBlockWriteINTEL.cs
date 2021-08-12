// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SubgroupBufferBlockIOINTEL })]
    public sealed record OpSubgroupBlockWriteINTEL : GroupInstruction
    {
        public ID Ptr { get; init; }
        public ID Data { get; init; }

        public override OpCode OpCode => OpCode.OpSubgroupBlockWriteINTEL;
        public override int WordCount => 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Ptr, Data };

        public OpSubgroupBlockWriteINTEL() {}

        private OpSubgroupBlockWriteINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Ptr = new ID(codes[i++]);
            Data = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Ptr.Value;
            codes[i++] = Data.Value;
        }
    }
}

