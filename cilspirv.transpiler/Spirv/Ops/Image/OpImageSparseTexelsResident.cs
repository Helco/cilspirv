// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.SparseResidency })]
    public sealed record OpImageSparseTexelsResident : ImageInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID ResidentCode { get; init; }

        public override OpCode OpCode => OpCode.OpImageSparseTexelsResident;
        public override int WordCount => 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, ResidentCode };

        public OpImageSparseTexelsResident() {}

        private OpImageSparseTexelsResident(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            ResidentCode = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = ResidentCode.Value;
        }
    }
}

