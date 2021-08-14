// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Kernel })]
    public sealed record OpAtomicFlagClear : AtomicInstruction
    {
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Semantics { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicFlagClear;
        public override int WordCount => 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Pointer, Memory, Semantics };

        public OpAtomicFlagClear() {}

        private OpAtomicFlagClear(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Semantics = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Pointer);
            codes[i++] = mapID(Memory);
            codes[i++] = mapID(Semantics);
        }
    }
}

