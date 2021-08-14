// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypeFloat : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public LiteralNumber Width { get; init; }

        public override OpCode OpCode => OpCode.OpTypeFloat;
        public override int WordCount => 1 + 1 + 1;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result };

        public OpTypeFloat() {}

        private OpTypeFloat(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            Width = (LiteralNumber)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Result);
            codes[i++] = Width.Value;
        }
    }
}

