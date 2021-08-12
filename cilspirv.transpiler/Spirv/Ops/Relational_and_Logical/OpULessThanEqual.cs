// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpULessThanEqual : RelationalAndLogicalInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Operand1 { get; init; }
        public ID Operand2 { get; init; }

        public override OpCode OpCode => OpCode.OpULessThanEqual;
        public override int WordCount => 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Operand1, Operand2 };

        public OpULessThanEqual() {}

        private OpULessThanEqual(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Operand1 = new ID(codes[i++]);
            Operand2 = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Operand1.Value;
            codes[i++] = Operand2.Value;
        }
    }
}

