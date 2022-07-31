// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSpecConstant : ConstantCreationInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ImmutableArray<LiteralNumber> Value { get; init; }

        public override OpCode OpCode => OpCode.OpSpecConstant;
        public override int WordCount => 1 + 1 + 1 + Value.Length + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result }.Concat(ExtraIDs);

        public OpSpecConstant() {}

        private OpSpecConstant(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Value = ReadContextDependentNumber(codes, ref i, end);
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
            Value.Select(v => v.Value).ToArray().CopyTo(codes.Slice(i)); i += Value.Length;
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
            writer.Write(string.Join("", Value.Select(n => n.Value.ToString("X8"))));
            DisassembleExtras(writer);
        }
    }
}

