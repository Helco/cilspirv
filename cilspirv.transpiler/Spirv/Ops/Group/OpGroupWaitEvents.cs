// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Kernel })]
    public sealed record OpGroupWaitEvents : GroupInstruction
    {
        public ID Execution { get; init; }
        public ID NumEvents { get; init; }
        public ID EventsList { get; init; }

        public override OpCode OpCode => OpCode.OpGroupWaitEvents;
        public override int WordCount => 1 + 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { Execution, NumEvents, EventsList }.Concat(ExtraIDs);

        public OpGroupWaitEvents() {}

        private OpGroupWaitEvents(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Execution = new ID(codes[i++]);
            NumEvents = new ID(codes[i++]);
            EventsList = new ID(codes[i++]);
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
            codes[i++] = mapID(Execution);
            codes[i++] = mapID(NumEvents);
            codes[i++] = mapID(EventsList);
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

