// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SubgroupShuffleINTEL })]
    public sealed record OpSubgroupShuffleXorINTEL : GroupInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Data { get; init; }
        public ID Value { get; init; }

        public override OpCode OpCode => OpCode.OpSubgroupShuffleXorINTEL;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Data, Value }.Concat(ExtraIDs);

        public OpSubgroupShuffleXorINTEL() {}

        private OpSubgroupShuffleXorINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Data = new ID(codes[i++]);
            Value = new ID(codes[i++]);
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
            codes[i++] = mapID(Data);
            codes[i++] = mapID(Value);
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

