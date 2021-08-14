// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypeImage : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public ID SampledType { get; init; }
        public Dim Dim { get; init; }
        public LiteralNumber Depth { get; init; }
        public LiteralNumber Arrayed { get; init; }
        public LiteralNumber MS { get; init; }
        public LiteralNumber Sampled { get; init; }
        public ImageFormat ImageFormat { get; init; }
        public AccessQualifier? AccessQualifier { get; init; }

        public override OpCode OpCode => OpCode.OpTypeImage;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + (AccessQualifier.HasValue ? 1 : 0) + ExtraWordCount;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result, SampledType }.Concat(ExtraIDs);

        public OpTypeImage() {}

        private OpTypeImage(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            SampledType = new ID(codes[i++]);
            Dim = (Dim)codes[i++];
            Depth = (LiteralNumber)codes[i++];
            Arrayed = (LiteralNumber)codes[i++];
            MS = (LiteralNumber)codes[i++];
            Sampled = (LiteralNumber)codes[i++];
            ImageFormat = (ImageFormat)codes[i++];
            if (i < end)
                AccessQualifier = (AccessQualifier)codes[i++];
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
            codes[i++] = mapID(Result);
            codes[i++] = mapID(SampledType);
            codes[i++] = (uint)Dim;
            codes[i++] = Depth.Value;
            codes[i++] = Arrayed.Value;
            codes[i++] = MS.Value;
            codes[i++] = Sampled.Value;
            codes[i++] = (uint)ImageFormat;
            if (AccessQualifier.HasValue)
            {
                codes[i++] = (uint)AccessQualifier.Value;
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

