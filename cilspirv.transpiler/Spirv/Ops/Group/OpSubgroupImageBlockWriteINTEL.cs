// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SubgroupImageBlockIOINTEL })]
    public sealed record OpSubgroupImageBlockWriteINTEL : GroupInstruction
    {
        public ID Image { get; init; }
        public ID Coordinate { get; init; }
        public ID Data { get; init; }

        public override OpCode OpCode => OpCode.OpSubgroupImageBlockWriteINTEL;
        public override int WordCount => 1 + 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { Image, Coordinate, Data }.Concat(ExtraIDs);

        public OpSubgroupImageBlockWriteINTEL() {}

        private OpSubgroupImageBlockWriteINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Image = new ID(codes[i++]);
            Coordinate = new ID(codes[i++]);
            Data = new ID(codes[i++]);
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
            codes[i++] = mapID(Image);
            codes[i++] = mapID(Coordinate);
            codes[i++] = mapID(Data);
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(Image);
            writer.Write(' ');
            writer.Write(Coordinate);
            writer.Write(' ');
            writer.Write(Data);
            DisassembleExtras(writer);
        }
    }
}

