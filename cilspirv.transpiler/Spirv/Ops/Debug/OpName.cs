// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpName : DebugInstruction
    {
        public ID Target { get; init; }
        public LiteralString Name { get; init; }

        public override OpCode OpCode => OpCode.OpName;
        public override int WordCount => 1 + 1 + Name.WordCount;

        public override IEnumerable<ID> AllIDs => new[] { Target };

        public OpName() {}

        private OpName(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Target = new ID(codes[i++]);
            Name = new LiteralString(codes, ref i);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Target.Value;
            Name.Write(codes, ref i);
        }
    }
}

