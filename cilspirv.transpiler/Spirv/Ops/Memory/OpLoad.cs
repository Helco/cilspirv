// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpLoad : MemoryInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Pointer { get; init; }
        public MemoryAccess? MemoryAccess3 { get; init; }

        public override OpCode OpCode => OpCode.OpLoad;
        public override int WordCount => 1 + 1 + 1 + 1 + (MemoryAccess3.HasValue ? 1 : 0);
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Pointer };

        public OpLoad() {}

        private OpLoad(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            if (i < end)
                MemoryAccess3 = (MemoryAccess)codes[i++];
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
            if (MemoryAccess3.HasValue)
            {
                codes[i++] = (uint)MemoryAccess3.Value;
            }
        }
    }
}

