// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpStore : MemoryInstruction
    {
        public ID Pointer { get; init; }
        public ID Object { get; init; }
        public MemoryAccess? MemoryAccess { get; init; }

        public override OpCode OpCode => OpCode.OpStore;
        public override int WordCount => 1 + 1 + 1 + (MemoryAccess.HasValue ? 1 : 0);

        public override IEnumerable<ID> AllIDs => new[] { Pointer, Object };

        public OpStore() {}

        private OpStore(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Pointer = new ID(codes[i++]);
            Object = new ID(codes[i++]);
            if (i < end)
                MemoryAccess = (MemoryAccess)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Pointer);
            codes[i++] = mapID(Object);
            if (MemoryAccess.HasValue)
            {
                codes[i++] = (uint)MemoryAccess.Value;
            }
        }
    }
}

