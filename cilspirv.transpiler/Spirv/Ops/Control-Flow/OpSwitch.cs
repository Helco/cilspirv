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
        public override int WordCount => 1 + 1 + 1 + 2 * Target.Length + ExtraWordCount;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { Selector, Default });
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
            i = end;
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
            codes[i++] = mapID(Selector);
            codes[i++] = mapID(Default);
            foreach (var x in Target)
            {
                codes[i++] = x.Item1.Value; codes[i++] = mapID(x.Item2);
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

