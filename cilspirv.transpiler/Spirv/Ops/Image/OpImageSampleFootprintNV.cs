// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.ImageFootprintNV }, Extensions = new[] { "SPV_NV_shader_image_footprint" })]
    public sealed record OpImageSampleFootprintNV : ImageInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID SampledImage { get; init; }
        public ID Coordinate { get; init; }
        public ID Granularity { get; init; }
        public ID Coarse { get; init; }
        public ImageOperands? ImageOperands { get; init; }

        public override OpCode OpCode => OpCode.OpImageSampleFootprintNV;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + (ImageOperands.HasValue ? 1 : 0) + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, SampledImage, Coordinate, Granularity, Coarse }.Concat(ExtraIDs);

        public OpImageSampleFootprintNV() {}

        private OpImageSampleFootprintNV(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            SampledImage = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Granularity = new ID(codes[i++]);
            Coarse = new ID(codes[i++]);
            if (i < end)
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
            codes[i++] = mapID(Granularity);
            codes[i++] = mapID(Coarse);
            if (ImageOperands.HasValue)
            {
                codes[i++] = (uint)ImageOperands.Value;
            }
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
            writer.Write(Granularity);
            writer.Write(' ');
            writer.Write(Coarse);
            writer.Write(' ');
            writer.Write(ImageOperands);
            DisassembleExtras(writer);
        }
    }
}

