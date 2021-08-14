// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.LongConstantCompositeINTEL })]
    public sealed record OpTypeStructContinuedINTEL : TypeDeclarationInstruction
    {
        public ImmutableArray<ID> Members { get; init; }

        public override OpCode OpCode => OpCode.OpTypeStructContinuedINTEL;
        public override int WordCount => 1 + Members.Length;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(Members);
                return result;
            }
        }

        public OpTypeStructContinuedINTEL() {}

        private OpTypeStructContinuedINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Members = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            foreach (var x in Members)
            {
                codes[i++] = mapID(x);
            }
        }
    }
}

