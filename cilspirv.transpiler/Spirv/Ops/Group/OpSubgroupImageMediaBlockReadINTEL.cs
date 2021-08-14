// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SubgroupImageMediaBlockIOINTEL })]
    public sealed record OpSubgroupImageMediaBlockReadINTEL : GroupInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Image { get; init; }
        public ID Coordinate { get; init; }
        public ID Width { get; init; }
        public ID Height { get; init; }

        public override OpCode OpCode => OpCode.OpSubgroupImageMediaBlockReadINTEL;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Image, Coordinate, Width, Height }.Concat(ExtraIDs);

        public OpSubgroupImageMediaBlockReadINTEL() {}

        private OpSubgroupImageMediaBlockReadINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Image = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Width = new ID(codes[i++]);
            Height = new ID(codes[i++]);
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
            codes[i++] = mapID(Image);
            codes[i++] = mapID(Coordinate);
            codes[i++] = mapID(Width);
            codes[i++] = mapID(Height);
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

