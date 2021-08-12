// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Shader, Capability.BitInstructions })]
    public sealed record OpBitFieldSExtract : BitInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Base { get; init; }
        public ID Offset { get; init; }
        public ID Count { get; init; }

        public override OpCode OpCode => OpCode.OpBitFieldSExtract;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Base, Offset, Count };

        public OpBitFieldSExtract() {}

        private OpBitFieldSExtract(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Base = new ID(codes[i++]);
            Offset = new ID(codes[i++]);
            Count = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Base.Value;
            codes[i++] = Offset.Value;
            codes[i++] = Count.Value;
        }
    }
}

