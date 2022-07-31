// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpLine : DebugInstruction
    {
        public ID File { get; init; }
        public LiteralNumber Line { get; init; }
        public LiteralNumber Column { get; init; }

        public override OpCode OpCode => OpCode.OpLine;
        public override int WordCount => 1 + 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { File }.Concat(ExtraIDs);

        public OpLine() {}

        private OpLine(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            File = new ID(codes[i++]);
            Line = (LiteralNumber)codes[i++];
            Column = (LiteralNumber)codes[i++];
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
            codes[i++] = mapID(File);
            codes[i++] = Line.Value;
            codes[i++] = Column.Value;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(File);
            writer.Write(' ');
            writer.Write(Line);
            writer.Write(' ');
            writer.Write(Column);
            DisassembleExtras(writer);
        }
    }
}

