// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.DeviceEnqueue })]
    public sealed record OpEnqueueMarker : DeviceSideEnqueueInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Queue { get; init; }
        public ID NumEvents { get; init; }
        public ID WaitEvents { get; init; }
        public ID RetEvent { get; init; }

        public override OpCode OpCode => OpCode.OpEnqueueMarker;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Queue, NumEvents, WaitEvents, RetEvent };

        public OpEnqueueMarker() {}

        private OpEnqueueMarker(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Queue = new ID(codes[i++]);
            NumEvents = new ID(codes[i++]);
            WaitEvents = new ID(codes[i++]);
            RetEvent = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Queue);
            codes[i++] = mapID(NumEvents);
            codes[i++] = mapID(WaitEvents);
            codes[i++] = mapID(RetEvent);
        }
    }
}

