// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpMemberDecorate : AnnotationInstruction
    {
        public ID StructureType { get; init; }
        public LiteralNumber Member { get; init; }
        public Decoration Decoration { get; init; }

        public override OpCode OpCode => OpCode.OpMemberDecorate;
        public override int WordCount => 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { StructureType };

        public OpMemberDecorate() {}

        private OpMemberDecorate(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            StructureType = new ID(codes[i++]);
            Member = (LiteralNumber)codes[i++];
            Decoration = (Decoration)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = StructureType.Value;
            codes[i++] = Member.Value;
            codes[i++] = (uint)Decoration;
        }
    }
}

