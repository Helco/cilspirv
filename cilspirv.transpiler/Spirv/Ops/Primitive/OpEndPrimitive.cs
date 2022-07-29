// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Geometry })]
    public sealed record OpEndPrimitive : PrimitiveInstruction
    {

        public override OpCode OpCode => OpCode.OpEndPrimitive;
        public override int WordCount => 1 + ExtraWordCount;


        public OpEndPrimitive() {}

        private OpEndPrimitive(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
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
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

    }
}

