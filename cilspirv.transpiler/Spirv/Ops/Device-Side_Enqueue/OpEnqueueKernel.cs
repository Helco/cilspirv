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
        public ID ResultType { get; init; }
        public ID Result { get; init; }
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
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + LocalSize.Length + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { ResultType, Result, Queue, Flags, NDRange, NumEvents, WaitEvents, RetEvent, Invoke, Param, ParamSize, ParamAlign });
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
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
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
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Queue);
            codes[i++] = mapID(Flags);
            codes[i++] = mapID(NDRange);
            codes[i++] = mapID(NumEvents);
            codes[i++] = mapID(WaitEvents);
            codes[i++] = mapID(RetEvent);
            codes[i++] = mapID(Invoke);
            codes[i++] = mapID(Param);
            codes[i++] = mapID(ParamSize);
            codes[i++] = mapID(ParamAlign);
            foreach (var x in LocalSize)
            {
                codes[i++] = mapID(x);
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

