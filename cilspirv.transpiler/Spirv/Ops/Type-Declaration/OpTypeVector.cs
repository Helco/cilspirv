// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypeVector : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public ID ComponentType { get; init; }
        public LiteralNumber ComponentCount { get; init; }

        public override OpCode OpCode => OpCode.OpTypeVector;
        public override int WordCount => 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result, ComponentType };

        public OpTypeVector() {}

        private OpTypeVector(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            ComponentType = new ID(codes[i++]);
            ComponentCount = (LiteralNumber)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result.Value;
            codes[i++] = ComponentType.Value;
            codes[i++] = ComponentCount.Value;
        }
    }
}

