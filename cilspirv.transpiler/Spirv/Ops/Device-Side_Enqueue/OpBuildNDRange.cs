// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.DeviceEnqueue })]
    public sealed record OpBuildNDRange : DeviceSideEnqueueInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID GlobalWorkSize { get; init; }
        public ID LocalWorkSize { get; init; }
        public ID GlobalWorkOffset { get; init; }

        public override OpCode OpCode => OpCode.OpBuildNDRange;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, GlobalWorkSize, LocalWorkSize, GlobalWorkOffset };

        public OpBuildNDRange() {}

        private OpBuildNDRange(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            GlobalWorkSize = new ID(codes[i++]);
            LocalWorkSize = new ID(codes[i++]);
            GlobalWorkOffset = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = GlobalWorkSize.Value;
            codes[i++] = LocalWorkSize.Value;
            codes[i++] = GlobalWorkOffset.Value;
        }
    }
}

