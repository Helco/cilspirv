// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpAtomicCompareExchange : AtomicInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Equal { get; init; }
        public ID Unequal { get; init; }
        public ID Value { get; init; }
        public ID Comparator { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicCompareExchange;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Pointer, Memory, Equal, Unequal, Value, Comparator };

        public OpAtomicCompareExchange() {}

        private OpAtomicCompareExchange(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Equal = new ID(codes[i++]);
            Unequal = new ID(codes[i++]);
            Value = new ID(codes[i++]);
            Comparator = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Pointer.Value;
            codes[i++] = Memory.Value;
            codes[i++] = Equal.Value;
            codes[i++] = Unequal.Value;
            codes[i++] = Value.Value;
            codes[i++] = Comparator.Value;
        }
    }
}

