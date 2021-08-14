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
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public SamplerAddressingMode SamplerAddressingMode { get; init; }
        public LiteralNumber Param { get; init; }
        public SamplerFilterMode SamplerFilterMode { get; init; }

        public override OpCode OpCode => OpCode.OpConstantSampler;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result }.Concat(ExtraIDs);

        public OpConstantSampler() {}

        private OpConstantSampler(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            SamplerAddressingMode = (SamplerAddressingMode)codes[i++];
            Param = (LiteralNumber)codes[i++];
            SamplerFilterMode = (SamplerFilterMode)codes[i++];
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
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = (uint)SamplerAddressingMode;
            codes[i++] = Param.Value;
            codes[i++] = (uint)SamplerFilterMode;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

