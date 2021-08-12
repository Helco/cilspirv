// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.LiteralSampler })]
    public sealed record OpConstantSampler : ConstantCreationInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public SamplerAddressingMode SamplerAddressingMode3 { get; init; }
        public LiteralNumber Param { get; init; }
        public SamplerFilterMode SamplerFilterMode4 { get; init; }

        public override OpCode OpCode => OpCode.OpConstantSampler;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2 };

        public OpConstantSampler() {}

        private OpConstantSampler(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            SamplerAddressingMode3 = (SamplerAddressingMode)codes[i++];
            Param = (LiteralNumber)codes[i++];
            SamplerFilterMode4 = (SamplerFilterMode)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = (uint)SamplerAddressingMode3;
            codes[i++] = Param.Value;
            codes[i++] = (uint)SamplerFilterMode4;
        }
    }
}

