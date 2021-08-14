// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.LongConstantCompositeINTEL })]
    public sealed record OpSpecConstantCompositeContinuedINTEL : ConstantCreationInstruction
    {
        public ImmutableArray<ID> Constituents { get; init; }

        public override OpCode OpCode => OpCode.OpSpecConstantCompositeContinuedINTEL;
        public override int WordCount => 1 + Constituents.Length + ExtraWordCount;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(Constituents);
                return result;
            }
        }

        public OpSpecConstantCompositeContinuedINTEL() {}

        private OpSpecConstantCompositeContinuedINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Constituents = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
                .ToImmutableArray();
            i = end;
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
            foreach (var x in Constituents)
            {
                codes[i++] = mapID(x);
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

