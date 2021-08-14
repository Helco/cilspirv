// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.1", Capabilities = new[] { Capability.SubgroupDispatch })]
    public sealed record OpGetKernelLocalSizeForSubgroupCount : DeviceSideEnqueueInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID SubgroupCount { get; init; }
        public ID Invoke { get; init; }
        public ID Param { get; init; }
        public ID ParamSize { get; init; }
        public ID ParamAlign { get; init; }

        public override OpCode OpCode => OpCode.OpGetKernelLocalSizeForSubgroupCount;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, SubgroupCount, Invoke, Param, ParamSize, ParamAlign };

        public OpGetKernelLocalSizeForSubgroupCount() {}

        private OpGetKernelLocalSizeForSubgroupCount(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            SubgroupCount = new ID(codes[i++]);
            Invoke = new ID(codes[i++]);
            Param = new ID(codes[i++]);
            ParamSize = new ID(codes[i++]);
            ParamAlign = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(SubgroupCount);
            codes[i++] = mapID(Invoke);
            codes[i++] = mapID(Param);
            codes[i++] = mapID(ParamSize);
            codes[i++] = mapID(ParamAlign);
        }
    }
}

