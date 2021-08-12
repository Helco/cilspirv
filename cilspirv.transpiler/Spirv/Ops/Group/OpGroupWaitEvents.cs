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
        public override int WordCount => 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Execution, NumEvents, EventsList };

        public OpGroupWaitEvents() {}

        private OpGroupWaitEvents(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Execution = new ID(codes[i++]);
            NumEvents = new ID(codes[i++]);
            EventsList = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Execution.Value;
            codes[i++] = NumEvents.Value;
            codes[i++] = EventsList.Value;
        }
    }
}

