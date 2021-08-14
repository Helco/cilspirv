// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSourceContinued : DebugInstruction
    {
        public LiteralString ContinuedSource { get; init; }

        public override OpCode OpCode => OpCode.OpSourceContinued;
        public override int WordCount => 1 + ContinuedSource.WordCount + ExtraWordCount;


        public OpSourceContinued() {}

        private OpSourceContinued(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ContinuedSource = new LiteralString(codes, ref i);
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
            ContinuedSource.Write(codes, ref i);
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

