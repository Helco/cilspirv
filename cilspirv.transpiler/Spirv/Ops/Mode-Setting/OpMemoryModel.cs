// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpMemoryModel : ModeSettingInstruction
    {
        public AddressingModel AddressingModel1 { get; init; }
        public MemoryModel MemoryModel2 { get; init; }

        public override OpCode OpCode => OpCode.OpMemoryModel;
        public override int WordCount => 1 + 1 + 1;


        public OpMemoryModel() {}

        private OpMemoryModel(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            AddressingModel1 = (AddressingModel)codes[i++];
            MemoryModel2 = (MemoryModel)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = (uint)AddressingModel1;
            codes[i++] = (uint)MemoryModel2;
        }
    }
}

