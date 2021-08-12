// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpLine : DebugInstruction
    {
        public ID File { get; init; }
        public LiteralNumber Line { get; init; }
        public LiteralNumber Column { get; init; }

        public override OpCode OpCode => OpCode.OpLine;
        public override int WordCount => 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { File };

        public OpLine() {}

        private OpLine(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            File = new ID(codes[i++]);
            Line = (LiteralNumber)codes[i++];
            Column = (LiteralNumber)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = File.Value;
            codes[i++] = Line.Value;
            codes[i++] = Column.Value;
        }
    }
}

