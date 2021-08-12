// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpMemoryModel : ModeSettingInstruction
    {
        public AddressingModel AddressingModel { get; init; }
        public MemoryModel MemoryModel { get; init; }

        public override OpCode OpCode => OpCode.OpMemoryModel;
        public override int WordCount => 1 + 1 + 1;


        public OpMemoryModel() {}

        private OpMemoryModel(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            AddressingModel = (AddressingModel)codes[i++];
            MemoryModel = (MemoryModel)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = (uint)AddressingModel;
            codes[i++] = (uint)MemoryModel;
        }
    }
}

