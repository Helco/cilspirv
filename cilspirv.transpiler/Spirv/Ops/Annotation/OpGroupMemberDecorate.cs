// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpGroupMemberDecorate : AnnotationInstruction
    {
        public ID DecorationGroup { get; init; }
        public ImmutableArray<(ID, LiteralNumber)> Targets { get; init; }

        public override OpCode OpCode => OpCode.OpGroupMemberDecorate;
        public override int WordCount => 1 + 1 + 2 * Targets.Length;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(Targets.Select(x => x.Item1));
                return result;
            }
        }

        public OpGroupMemberDecorate() {}

        private OpGroupMemberDecorate(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            DecorationGroup = new ID(codes[i++]);
            Targets = Enumerable.Repeat(0, (end - i) / 2)
                .Select(_ => (new ID(codes[i++]), new LiteralNumber(codes[i++])))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(DecorationGroup);
            foreach (var x in Targets)
            {
                codes[i++] = mapID(x.Item1); codes[i++] = x.Item2.Value;
            }
        }
    }
}

