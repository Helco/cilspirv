// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.DeviceEnqueue })]
    public sealed record OpSetUserEventStatus : DeviceSideEnqueueInstruction
    {
        public ID Event { get; init; }
        public ID Status { get; init; }

        public override OpCode OpCode => OpCode.OpSetUserEventStatus;
        public override int WordCount => 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Event, Status };

        public OpSetUserEventStatus() {}

        private OpSetUserEventStatus(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Event = new ID(codes[i++]);
            Status = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Event.Value;
            codes[i++] = Status.Value;
        }
    }
}

