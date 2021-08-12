// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SubgroupImageMediaBlockIOINTEL })]
    public sealed record OpSubgroupImageMediaBlockWriteINTEL : GroupInstruction
    {
        public ID Image { get; init; }
        public ID Coordinate { get; init; }
        public ID Width { get; init; }
        public ID Height { get; init; }
        public ID Data { get; init; }

        public override OpCode OpCode => OpCode.OpSubgroupImageMediaBlockWriteINTEL;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Image, Coordinate, Width, Height, Data };

        public OpSubgroupImageMediaBlockWriteINTEL() {}

        private OpSubgroupImageMediaBlockWriteINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Image = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Width = new ID(codes[i++]);
            Height = new ID(codes[i++]);
            Data = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Image.Value;
            codes[i++] = Coordinate.Value;
            codes[i++] = Width.Value;
            codes[i++] = Height.Value;
            codes[i++] = Data.Value;
        }
    }
}

