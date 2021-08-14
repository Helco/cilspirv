// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpIMul : ArithmeticInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Operand1 { get; init; }
        public ID Operand2 { get; init; }

        public override OpCode OpCode => OpCode.OpIMul;
        public override int WordCount => 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Operand1, Operand2 };

        public OpIMul() {}

        private OpIMul(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Operand1 = new ID(codes[i++]);
            Operand2 = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Operand1);
            codes[i++] = mapID(Operand2);
        }
    }
}

