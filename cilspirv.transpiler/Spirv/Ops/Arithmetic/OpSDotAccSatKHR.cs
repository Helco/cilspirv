// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.DotProductKHR })]
    public sealed record OpSDotAccSatKHR : ArithmeticInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Vector1 { get; init; }
        public ID Vector2 { get; init; }
        public ID Accumulator { get; init; }
        public PackedVectorFormat? PackedVectorFormat { get; init; }

        public override OpCode OpCode => OpCode.OpSDotAccSatKHR;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + (PackedVectorFormat.HasValue ? 1 : 0) + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Vector1, Vector2, Accumulator }.Concat(ExtraIDs);

        public OpSDotAccSatKHR() {}

        private OpSDotAccSatKHR(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Vector1 = new ID(codes[i++]);
            Vector2 = new ID(codes[i++]);
            Accumulator = new ID(codes[i++]);
            if (i < end)
                PackedVectorFormat = (PackedVectorFormat)codes[i++];
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
            codes[i++] = mapID(Accumulator);
            if (PackedVectorFormat.HasValue)
            {
                codes[i++] = (uint)PackedVectorFormat.Value;
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

