// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpAtomicCompareExchange : AtomicInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Equal { get; init; }
        public ID Unequal { get; init; }
        public ID Value { get; init; }
        public ID Comparator { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicCompareExchange;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Pointer, Memory, Equal, Unequal, Value, Comparator }.Concat(ExtraIDs);

        public OpAtomicCompareExchange() {}

        private OpAtomicCompareExchange(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Equal = new ID(codes[i++]);
            Unequal = new ID(codes[i++]);
            Value = new ID(codes[i++]);
            Comparator = new ID(codes[i++]);
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
            codes[i++] = mapID(Pointer);
            codes[i++] = mapID(Memory);
            codes[i++] = mapID(Equal);
            codes[i++] = mapID(Unequal);
            codes[i++] = mapID(Value);
            codes[i++] = mapID(Comparator);
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
            writer.Write(Pointer);
            writer.Write(' ');
            writer.Write(Memory);
            writer.Write(' ');
            writer.Write(Equal);
            writer.Write(' ');
            writer.Write(Unequal);
            writer.Write(' ');
            writer.Write(Value);
            writer.Write(' ');
            writer.Write(Comparator);
            DisassembleExtras(writer);
        }
    }
}

