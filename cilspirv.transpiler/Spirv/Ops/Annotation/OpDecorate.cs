// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpDecorate : AnnotationInstruction
    {
        public ID Target { get; init; }
        public Decoration Decoration { get; init; }

        public override OpCode OpCode => OpCode.OpDecorate;
        public override int WordCount => 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { Target }.Concat(ExtraIDs);

        public OpDecorate() {}

        private OpDecorate(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Target = new ID(codes[i++]);
            Decoration = (Decoration)codes[i++];
            ExtraOperands = codes.Skip(i).Take(end - i)
                .Select(x => new ExtraOperand(x))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Target);
            codes[i++] = (uint)Decoration;
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

