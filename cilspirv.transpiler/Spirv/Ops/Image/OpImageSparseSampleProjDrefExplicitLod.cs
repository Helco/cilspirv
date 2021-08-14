// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SparseResidency })]
    public sealed record OpImageSparseSampleProjDrefExplicitLod : ImageInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID SampledImage { get; init; }
        public ID Coordinate { get; init; }
        public ID Dref { get; init; }
        public ImageOperands ImageOperands { get; init; }

        public override OpCode OpCode => OpCode.OpImageSparseSampleProjDrefExplicitLod;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, SampledImage, Coordinate, Dref };

        public OpImageSparseSampleProjDrefExplicitLod() {}

        private OpImageSparseSampleProjDrefExplicitLod(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            SampledImage = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Dref = new ID(codes[i++]);
            ImageOperands = (ImageOperands)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(SampledImage);
            codes[i++] = mapID(Coordinate);
            codes[i++] = mapID(Dref);
            codes[i++] = (uint)ImageOperands;
        }
    }
}

