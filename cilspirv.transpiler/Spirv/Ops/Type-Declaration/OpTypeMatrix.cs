// This file was generated. Do not modify.
using System;
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
        public override int WordCount => 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result, ColumnType };

        public OpTypeMatrix() {}

        private OpTypeMatrix(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            ColumnType = new ID(codes[i++]);
            ColumnCount = (LiteralNumber)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result.Value;
            codes[i++] = ColumnType.Value;
            codes[i++] = ColumnCount.Value;
        }
    }
}

