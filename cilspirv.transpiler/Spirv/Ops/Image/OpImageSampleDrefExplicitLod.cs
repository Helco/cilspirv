// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Shader })]
    public sealed record OpImageSampleDrefExplicitLod : ImageInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID SampledImage { get; init; }
        public ID Coordinate { get; init; }
        public ID Dref { get; init; }
        public ImageOperands ImageOperands { get; init; }

        public override OpCode OpCode => OpCode.OpImageSampleDrefExplicitLod;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, SampledImage, Coordinate, Dref }.Concat(ExtraIDs);

        public OpImageSampleDrefExplicitLod() {}

        private OpImageSampleDrefExplicitLod(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            SampledImage = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Dref = new ID(codes[i++]);
            ImageOperands = (ImageOperands)codes[i++];
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
            codes[i++] = mapID(SampledImage);
            codes[i++] = mapID(Coordinate);
            codes[i++] = mapID(Dref);
            codes[i++] = (uint)ImageOperands;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(ResultType);
            writer.Write(' ');
            writer.Write(SampledImage);
            writer.Write(' ');
            writer.Write(Coordinate);
            writer.Write(' ');
            writer.Write(Dref);
            writer.Write(' ');
            writer.Write(ImageOperands);
            DisassembleExtras(writer);
        }
    }
}

