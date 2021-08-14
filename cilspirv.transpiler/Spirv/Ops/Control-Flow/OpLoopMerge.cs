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
        public override int WordCount => 1 + 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { MergeBlock, ContinueTarget }.Concat(ExtraIDs);

        public OpLoopMerge() {}

        private OpLoopMerge(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            MergeBlock = new ID(codes[i++]);
            ContinueTarget = new ID(codes[i++]);
            LoopControl = (LoopControl)codes[i++];
            ExtraOperands = codes.Skip(i).Take(end - i)
                .Select(x => new ExtraOperand(x))
                .ToImmutableArray();
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
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

