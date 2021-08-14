// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpConstant : ConstantCreationInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ImmutableArray<LiteralNumber> Value { get; init; }

        public override OpCode OpCode => OpCode.OpConstant;
        public override int WordCount => 1 + 1 + 1 + Value.Length;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result };

        public OpConstant() {}

        private OpConstant(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Value = codes.Skip(i).Take(end - i).Select(n => (LiteralNumber)n).ToImmutableArray();
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
        }
    }
}
