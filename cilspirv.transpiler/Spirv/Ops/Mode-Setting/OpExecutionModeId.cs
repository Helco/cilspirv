// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.2")]
    public sealed record OpExecutionModeId : ModeSettingInstruction
    {
        public ID EntryPoint { get; init; }
        public ExecutionMode Mode { get; init; }

        public override OpCode OpCode => OpCode.OpExecutionModeId;
        public override int WordCount => 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { EntryPoint };

        public OpExecutionModeId() {}

        private OpExecutionModeId(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            EntryPoint = new ID(codes[i++]);
            Mode = (ExecutionMode)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = EntryPoint.Value;
            codes[i++] = (uint)Mode;
        }
    }
}

