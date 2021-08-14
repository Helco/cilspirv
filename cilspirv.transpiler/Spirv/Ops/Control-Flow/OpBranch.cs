// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpBranch : ControlFlowInstruction
    {
        public ID TargetLabel { get; init; }

        public override OpCode OpCode => OpCode.OpBranch;
        public override int WordCount => 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { TargetLabel };

        public OpBranch() {}

        private OpBranch(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            TargetLabel = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(TargetLabel);
        }
    }
}

