// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpCapability : ModeSettingInstruction
    {
        public Capability Capability { get; init; }

        public override OpCode OpCode => OpCode.OpCapability;
        public override int WordCount => 1 + 1;


        public OpCapability() {}

        private OpCapability(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Capability = (Capability)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = (uint)Capability;
        }
    }
}
