// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpImageWrite : ImageInstruction
    {
        public ID Image { get; init; }
        public ID Coordinate { get; init; }
        public ID Texel { get; init; }
        public ImageOperands? ImageOperands { get; init; }

        public override OpCode OpCode => OpCode.OpImageWrite;
        public override int WordCount => 1 + 1 + 1 + 1 + (ImageOperands.HasValue ? 1 : 0);

        public override IEnumerable<ID> AllIDs => new[] { Image, Coordinate, Texel };

        public OpImageWrite() {}

        private OpImageWrite(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Image = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Texel = new ID(codes[i++]);
            if (i < end)
                ImageOperands = (ImageOperands)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Image);
            codes[i++] = mapID(Coordinate);
            codes[i++] = mapID(Texel);
            if (ImageOperands.HasValue)
            {
                codes[i++] = (uint)ImageOperands.Value;
            }
        }
    }
}

