// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypeStruct : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public ImmutableArray<ID> Members { get; init; }

        public override OpCode OpCode => OpCode.OpTypeStruct;
        public override int WordCount => 1 + 1 + Members.Length;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(Members);
                return result;
            }
        }

        public OpTypeStruct() {}

        private OpTypeStruct(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            Members = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result.Value;
            foreach (var x in Members)
            {
                codes[i++] = x.Value;
            }
        }
    }
}

