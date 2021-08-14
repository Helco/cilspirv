// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpAtomicUMin : AtomicInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Semantics { get; init; }
        public ID Value { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicUMin;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Pointer, Memory, Semantics, Value };

        public OpAtomicUMin() {}

        private OpAtomicUMin(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Semantics = new ID(codes[i++]);
            Value = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Pointer);
            codes[i++] = mapID(Memory);
            codes[i++] = mapID(Semantics);
            codes[i++] = mapID(Value);
        }
    }
}

