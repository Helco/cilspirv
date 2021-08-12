// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSelect : RelationalAndLogicalInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Condition { get; init; }
        public ID Object1 { get; init; }
        public ID Object2 { get; init; }

        public override OpCode OpCode => OpCode.OpSelect;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Condition, Object1, Object2 };

        public OpSelect() {}

        private OpSelect(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Condition = new ID(codes[i++]);
            Object1 = new ID(codes[i++]);
            Object2 = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = Condition.Value;
            codes[i++] = Object1.Value;
            codes[i++] = Object2.Value;
        }
    }
}

