// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpPhi : ControlFlowInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ImmutableArray<(ID, ID)> Operands { get; init; }

        public override OpCode OpCode => OpCode.OpPhi;
        public override int WordCount => 1 + 1 + 1 + 2 * Operands.Length;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

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
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Operands = Enumerable.Repeat(0, (end - i) / 2)
                .Select(_ => (new ID(codes[i++]), new ID(codes[i++])))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            foreach (var x in Operands)
            {
                codes[i++] = mapID(x.Item1); codes[i++] = mapID(x.Item2);
            }
        }
    }
}

