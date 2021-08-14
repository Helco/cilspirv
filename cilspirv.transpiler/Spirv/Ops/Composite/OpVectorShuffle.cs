// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpVectorShuffle : CompositeInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Vector1 { get; init; }
        public ID Vector2 { get; init; }
        public ImmutableArray<LiteralNumber> Components { get; init; }

        public override OpCode OpCode => OpCode.OpVectorShuffle;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + Components.Length + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Vector1, Vector2 }.Concat(ExtraIDs);

        public OpVectorShuffle() {}

        private OpVectorShuffle(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Vector1 = new ID(codes[i++]);
            Vector2 = new ID(codes[i++]);
            Components = codes.Skip(i).Take(end - i)
                .Select(x => (LiteralNumber)x)
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
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Vector1);
            codes[i++] = mapID(Vector2);
            foreach (var x in Components)
            {
                codes[i++] = x.Value;
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

