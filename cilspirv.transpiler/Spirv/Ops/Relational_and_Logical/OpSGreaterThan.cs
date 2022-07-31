// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSGreaterThan : RelationalAndLogicalInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Operand1 { get; init; }
        public ID Operand2 { get; init; }

        public override OpCode OpCode => OpCode.OpSGreaterThan;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Operand1, Operand2 }.Concat(ExtraIDs);

        public OpSGreaterThan() {}

        private OpSGreaterThan(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Operand1 = new ID(codes[i++]);
            Operand2 = new ID(codes[i++]);
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
            codes[i++] = mapID(Operand1);
            codes[i++] = mapID(Operand2);
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
            writer.Write(Operand1);
            writer.Write(' ');
            writer.Write(Operand2);
            DisassembleExtras(writer);
        }
    }
}

