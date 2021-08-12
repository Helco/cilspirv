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
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID GlobalWorkSize { get; init; }
        public ID LocalWorkSize { get; init; }
        public ID GlobalWorkOffset { get; init; }

        public override OpCode OpCode => OpCode.OpBuildNDRange;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, GlobalWorkSize, LocalWorkSize, GlobalWorkOffset };

        public OpBuildNDRange() {}

        private OpBuildNDRange(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
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
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = GlobalWorkSize.Value;
            codes[i++] = LocalWorkSize.Value;
            codes[i++] = GlobalWorkOffset.Value;
        }
    }
}

