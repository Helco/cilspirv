// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Kernel })]
    public sealed record OpTypeEvent : TypeDeclarationInstruction
    {
        public ID Result { get; init; }

        public override OpCode OpCode => OpCode.OpTypeEvent;
        public override int WordCount => 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result }.Concat(ExtraIDs);

        public OpTypeEvent() {}

        private OpTypeEvent(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
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
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

