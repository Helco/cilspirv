// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpAtomicIDecrement : AtomicInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Semantics { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicIDecrement;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Pointer, Memory, Semantics };

        public OpAtomicIDecrement() {}

        private OpAtomicIDecrement(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Semantics = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = Pointer.Value;
            codes[i++] = Memory.Value;
            codes[i++] = Semantics.Value;
        }
    }
}

