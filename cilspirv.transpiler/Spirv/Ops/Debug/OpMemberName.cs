// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpMemberName : DebugInstruction
    {
        public ID Type { get; init; }
        public LiteralNumber Member { get; init; }
        public LiteralString Name { get; init; }

        public override OpCode OpCode => OpCode.OpMemberName;
        public override int WordCount => 1 + 1 + 1 + Name.WordCount + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { Type }.Concat(ExtraIDs);

        public OpMemberName() {}

        private OpMemberName(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Type = new ID(codes[i++]);
            Member = (LiteralNumber)codes[i++];
            Name = new LiteralString(codes, ref i);
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
            codes[i++] = mapID(Type);
            codes[i++] = Member.Value;
            Name.Write(codes, ref i);
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(Type);
            writer.Write(' ');
            writer.Write(Member);
            writer.Write(' ');
            writer.Write(Name);
            DisassembleExtras(writer);
        }
    }
}

