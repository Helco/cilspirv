// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Matrix })]
    public sealed record OpTypeMatrix : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public ID ColumnType { get; init; }
        public LiteralNumber ColumnCount { get; init; }

        public override OpCode OpCode => OpCode.OpTypeMatrix;
        public override int WordCount => 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result, ColumnType }.Concat(ExtraIDs);

        public OpTypeMatrix() {}

        private OpTypeMatrix(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            ColumnType = new ID(codes[i++]);
            ColumnCount = (LiteralNumber)codes[i++];
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
            codes[i++] = mapID(ColumnType);
            codes[i++] = ColumnCount.Value;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(ColumnType);
            writer.Write(' ');
            writer.Write(ColumnCount);
        }
    }
}

