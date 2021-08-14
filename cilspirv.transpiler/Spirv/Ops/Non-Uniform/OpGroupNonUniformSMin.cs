// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.3", Capabilities = new[] { Capability.GroupNonUniformArithmetic, Capability.GroupNonUniformClustered, Capability.GroupNonUniformPartitionedNV })]
    public sealed record OpGroupNonUniformSMin : NonUniformInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Execution { get; init; }
        public GroupOperation Operation { get; init; }
        public ID Value { get; init; }
        public ID? ClusterSize { get; init; }

        public override OpCode OpCode => OpCode.OpGroupNonUniformSMin;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + (ClusterSize.HasValue ? 1 : 0) + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { ResultType, Result, Execution, Value });
                result = result.Concat(new[] { ClusterSize }
                    .Where(o => o.HasValue)
                    .Select(o => o!.Value));
                return result;
            }
        }

        public OpGroupNonUniformSMin() {}

        private OpGroupNonUniformSMin(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Execution = new ID(codes[i++]);
            Operation = (GroupOperation)codes[i++];
            Value = new ID(codes[i++]);
            if (i < end)
                ClusterSize = new ID(codes[i++]);
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
            codes[i++] = mapID(Execution);
            codes[i++] = (uint)Operation;
            codes[i++] = mapID(Value);
            if (ClusterSize.HasValue)
            {
                codes[i++] = mapID(ClusterSize.Value);
            }
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

