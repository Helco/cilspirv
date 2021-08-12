// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpAtomicStore : AtomicInstruction
    {
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Semantics { get; init; }
        public ID Value { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicStore;
        public override int WordCount => 1 + 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Pointer, Memory, Semantics, Value };

        public OpAtomicStore() {}

        private OpAtomicStore(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Semantics = new ID(codes[i++]);
            Value = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Pointer.Value;
            codes[i++] = Memory.Value;
            codes[i++] = Semantics.Value;
            codes[i++] = Value.Value;
        }
    }
}

