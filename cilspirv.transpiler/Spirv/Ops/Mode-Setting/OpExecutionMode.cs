// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpExecutionMode : ModeSettingInstruction
    {
        public ID EntryPoint { get; init; }
        public ExecutionMode Mode { get; init; }

        public override OpCode OpCode => OpCode.OpExecutionMode;
        public override int WordCount => 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { EntryPoint };

        public OpExecutionMode() {}

        private OpExecutionMode(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            EntryPoint = new ID(codes[i++]);
            Mode = (ExecutionMode)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(EntryPoint);
            codes[i++] = (uint)Mode;
        }
    }
}

