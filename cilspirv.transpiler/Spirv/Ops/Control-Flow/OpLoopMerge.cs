// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpLoopMerge : ControlFlowInstruction
    {
        public ID MergeBlock { get; init; }
        public ID ContinueTarget { get; init; }
        public LoopControl LoopControl { get; init; }

        public override OpCode OpCode => OpCode.OpLoopMerge;
        public override int WordCount => 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { MergeBlock, ContinueTarget };

        public OpLoopMerge() {}

        private OpLoopMerge(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            MergeBlock = new ID(codes[i++]);
            ContinueTarget = new ID(codes[i++]);
            LoopControl = (LoopControl)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(MergeBlock);
            codes[i++] = mapID(ContinueTarget);
            codes[i++] = (uint)LoopControl;
        }
    }
}

