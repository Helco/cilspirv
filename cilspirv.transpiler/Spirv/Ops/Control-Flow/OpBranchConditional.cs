// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpBranchConditional : ControlFlowInstruction
    {
        public ID Condition { get; init; }
        public ID TrueLabel { get; init; }
        public ID FalseLabel { get; init; }
        public ImmutableArray<LiteralNumber> Branchweights { get; init; }

        public override OpCode OpCode => OpCode.OpBranchConditional;
        public override int WordCount => 1 + 1 + 1 + 1 + Branchweights.Length;

        public override IEnumerable<ID> AllIDs => new[] { Condition, TrueLabel, FalseLabel };

        public OpBranchConditional() {}

        private OpBranchConditional(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Condition = new ID(codes[i++]);
            TrueLabel = new ID(codes[i++]);
            FalseLabel = new ID(codes[i++]);
            Branchweights = codes.Skip(i).Take(end - i)
                .Select(x => (LiteralNumber)x)
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Condition);
            codes[i++] = mapID(TrueLabel);
            codes[i++] = mapID(FalseLabel);
            foreach (var x in Branchweights)
            {
                codes[i++] = x.Value;
            }
        }
    }
}
