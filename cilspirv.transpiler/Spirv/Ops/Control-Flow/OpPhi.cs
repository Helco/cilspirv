// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpPhi : ControlFlowInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ImmutableArray<(ID, ID)> Operands { get; init; }

        public override OpCode OpCode => OpCode.OpPhi;
        public override int WordCount => 1 + 1 + 1 + 2 * Operands.Length;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(Operands.Select(x => x.Item1));
                result = result.Concat(Operands.Select(x => x.Item2));
                return result;
            }
        }

        public OpPhi() {}

        private OpPhi(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Operands = Enumerable.Repeat(0, (end - i) / 2)
                .Select(_ => (new ID(codes[i++]), new ID(codes[i++])))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            foreach (var x in Operands)
            {
                codes[i++] = x.Item1.Value; codes[i++] = x.Item2.Value;
            }
        }
    }
}

