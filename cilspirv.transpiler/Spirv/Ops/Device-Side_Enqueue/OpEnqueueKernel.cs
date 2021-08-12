// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.DeviceEnqueue })]
    public sealed record OpEnqueueKernel : DeviceSideEnqueueInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Queue { get; init; }
        public ID Flags { get; init; }
        public ID NDRange { get; init; }
        public ID NumEvents { get; init; }
        public ID WaitEvents { get; init; }
        public ID RetEvent { get; init; }
        public ID Invoke { get; init; }
        public ID Param { get; init; }
        public ID ParamSize { get; init; }
        public ID ParamAlign { get; init; }
        public ImmutableArray<ID> LocalSize { get; init; }

        public override OpCode OpCode => OpCode.OpEnqueueKernel;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + LocalSize.Length;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(LocalSize);
                return result;
            }
        }

        public OpEnqueueKernel() {}

        private OpEnqueueKernel(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Queue = new ID(codes[i++]);
            Flags = new ID(codes[i++]);
            NDRange = new ID(codes[i++]);
            NumEvents = new ID(codes[i++]);
            WaitEvents = new ID(codes[i++]);
            RetEvent = new ID(codes[i++]);
            Invoke = new ID(codes[i++]);
            Param = new ID(codes[i++]);
            ParamSize = new ID(codes[i++]);
            ParamAlign = new ID(codes[i++]);
            LocalSize = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Queue.Value;
            codes[i++] = Flags.Value;
            codes[i++] = NDRange.Value;
            codes[i++] = NumEvents.Value;
            codes[i++] = WaitEvents.Value;
            codes[i++] = RetEvent.Value;
            codes[i++] = Invoke.Value;
            codes[i++] = Param.Value;
            codes[i++] = ParamSize.Value;
            codes[i++] = ParamAlign.Value;
            foreach (var x in LocalSize)
            {
                codes[i++] = x.Value;
            }
        }
    }
}

