// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Matrix })]
    public sealed record OpOuterProduct : ArithmeticInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Vector1 { get; init; }
        public ID Vector2 { get; init; }

        public override OpCode OpCode => OpCode.OpOuterProduct;
        public override int WordCount => 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Vector1, Vector2 };

        public OpOuterProduct() {}

        private OpOuterProduct(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Vector1 = new ID(codes[i++]);
            Vector2 = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Vector1);
            codes[i++] = mapID(Vector2);
        }
    }
}

