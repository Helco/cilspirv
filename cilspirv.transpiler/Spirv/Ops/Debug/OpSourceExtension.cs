// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSourceExtension : DebugInstruction
    {
        public LiteralString Extension { get; init; }

        public override OpCode OpCode => OpCode.OpSourceExtension;
        public override int WordCount => 1 + Extension.WordCount;


        public OpSourceExtension() {}

        private OpSourceExtension(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Extension = new LiteralString(codes, ref i);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            Extension.Write(codes, ref i);
        }
    }
}

