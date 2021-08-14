// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.DeviceEnqueue })]
    public sealed record OpCaptureEventProfilingInfo : DeviceSideEnqueueInstruction
    {
        public ID Event { get; init; }
        public ID ProfilingInfo { get; init; }
        public ID Value { get; init; }

        public override OpCode OpCode => OpCode.OpCaptureEventProfilingInfo;
        public override int WordCount => 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Event, ProfilingInfo, Value };

        public OpCaptureEventProfilingInfo() {}

        private OpCaptureEventProfilingInfo(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Event = new ID(codes[i++]);
            ProfilingInfo = new ID(codes[i++]);
            Value = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Event);
            codes[i++] = mapID(ProfilingInfo);
            codes[i++] = mapID(Value);
        }
    }
}
