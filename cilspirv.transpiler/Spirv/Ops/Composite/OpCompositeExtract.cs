// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpCompositeExtract : CompositeInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Composite { get; init; }
        public ImmutableArray<LiteralNumber> Indexes { get; init; }

        public override OpCode OpCode => OpCode.OpCompositeExtract;
        public override int WordCount => 1 + 1 + 1 + 1 + Indexes.Length + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Composite }.Concat(ExtraIDs);

        public OpCompositeExtract() {}

        private OpCompositeExtract(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Composite = new ID(codes[i++]);
            Indexes = codes.Skip(i).Take(end - i)
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
            codes[i++] = mapID(Composite);
            foreach (var x in Indexes)
            {
                codes[i++] = x.Value;
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

