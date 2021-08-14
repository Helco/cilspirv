// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpExtension : ExtensionInstruction
    {
        public LiteralString Name { get; init; }

        public override OpCode OpCode => OpCode.OpExtension;
        public override int WordCount => 1 + Name.WordCount;


        public OpExtension() {}

        private OpExtension(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Name = new LiteralString(codes, ref i);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            Name.Write(codes, ref i);
        }
    }
}

