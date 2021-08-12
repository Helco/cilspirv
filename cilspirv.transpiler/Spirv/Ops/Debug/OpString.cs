// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpString : DebugInstruction
    {
        public ID Result { get; init; }
        public LiteralString String { get; init; }

        public override OpCode OpCode => OpCode.OpString;
        public override int WordCount => 1 + 1 + String.WordCount;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result };

        public OpString() {}

        private OpString(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            String = new LiteralString(codes, ref i);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result.Value;
            String.Write(codes, ref i);
        }
    }
}

