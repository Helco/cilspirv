// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpGroupDecorate : AnnotationInstruction
    {
        public ID DecorationGroup { get; init; }
        public ImmutableArray<ID> Targets { get; init; }

        public override OpCode OpCode => OpCode.OpGroupDecorate;
        public override int WordCount => 1 + 1 + Targets.Length;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(Targets);
                return result;
            }
        }

        public OpGroupDecorate() {}

        private OpGroupDecorate(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            DecorationGroup = new ID(codes[i++]);
            Targets = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
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
                codes[i++] = mapID(x);
            }
        }
    }
}

