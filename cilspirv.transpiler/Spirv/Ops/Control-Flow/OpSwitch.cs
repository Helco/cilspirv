// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSwitch : ControlFlowInstruction
    {
        public ID Selector { get; init; }
        public ID Default { get; init; }
        public ImmutableArray<(LiteralNumber, ID)> Target { get; init; }

        public override OpCode OpCode => OpCode.OpSwitch;
        public override int WordCount => 1 + 1 + 1 + 2 * Target.Length;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(Target.Select(x => x.Item2));
                return result;
            }
        }

        public OpSwitch() {}

        private OpSwitch(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Selector = new ID(codes[i++]);
            Default = new ID(codes[i++]);
            Target = Enumerable.Repeat(0, (end - i) / 2)
                .Select(_ => (new LiteralNumber(codes[i++]), new ID(codes[i++])))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Selector.Value;
            codes[i++] = Default.Value;
            foreach (var x in Target)
            {
                codes[i++] = x.Item1.Value; codes[i++] = x.Item2.Value;
            }
        }
    }
}

