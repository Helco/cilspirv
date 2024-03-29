// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSelect : RelationalAndLogicalInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Condition { get; init; }
        public ID Object1 { get; init; }
        public ID Object2 { get; init; }

        public override OpCode OpCode => OpCode.OpSelect;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Condition, Object1, Object2 }.Concat(ExtraIDs);

        public OpSelect() {}

        private OpSelect(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Condition = new ID(codes[i++]);
            Object1 = new ID(codes[i++]);
            Object2 = new ID(codes[i++]);
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
            codes[i++] = mapID(Condition);
            codes[i++] = mapID(Object1);
            codes[i++] = mapID(Object2);
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
            writer.Write(Condition);
            writer.Write(' ');
            writer.Write(Object1);
            writer.Write(' ');
            writer.Write(Object2);
            DisassembleExtras(writer);
        }
    }
}

