// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.DotProductKHR })]
    public sealed record OpUDotKHR : ArithmeticInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Vector1 { get; init; }
        public ID Vector2 { get; init; }
        public PackedVectorFormat? PackedVectorFormat { get; init; }

        public override OpCode OpCode => OpCode.OpUDotKHR;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + (PackedVectorFormat.HasValue ? 1 : 0);
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Vector1, Vector2 };

        public OpUDotKHR() {}

        private OpUDotKHR(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Vector1 = new ID(codes[i++]);
            Vector2 = new ID(codes[i++]);
            if (i < end)
                PackedVectorFormat = (PackedVectorFormat)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Vector1.Value;
            codes[i++] = Vector2.Value;
            if (PackedVectorFormat.HasValue)
            {
                codes[i++] = (uint)PackedVectorFormat.Value;
            }
        }
    }
}

