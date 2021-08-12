// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.3", Capabilities = new[] { Capability.GroupNonUniformArithmetic, Capability.GroupNonUniformClustered, Capability.GroupNonUniformPartitionedNV })]
    public sealed record OpGroupNonUniformBitwiseAnd : NonUniformInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Execution { get; init; }
        public GroupOperation Operation { get; init; }
        public ID Value { get; init; }
        public ID? ClusterSize { get; init; }

        public override OpCode OpCode => OpCode.OpGroupNonUniformBitwiseAnd;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + (ClusterSize.HasValue ? 1 : 0);
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(new[] { ClusterSize }
                    .Where(o => o.HasValue)
                    .Select(o => o!.Value));
                return result;
            }
        }

        public OpGroupNonUniformBitwiseAnd() {}

        private OpGroupNonUniformBitwiseAnd(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Execution = new ID(codes[i++]);
            Operation = (GroupOperation)codes[i++];
            Value = new ID(codes[i++]);
            if (i < end)
                ClusterSize = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Execution.Value;
            codes[i++] = (uint)Operation;
            codes[i++] = Value.Value;
            if (ClusterSize.HasValue)
            {
                codes[i++] = ClusterSize.Value.Value;
            }
        }
    }
}

