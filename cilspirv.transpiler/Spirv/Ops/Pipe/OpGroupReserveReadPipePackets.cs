// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Pipes })]
    public sealed record OpGroupReserveReadPipePackets : PipeInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Execution { get; init; }
        public ID Pipe { get; init; }
        public ID NumPackets { get; init; }
        public ID PacketSize { get; init; }
        public ID PacketAlignment { get; init; }

        public override OpCode OpCode => OpCode.OpGroupReserveReadPipePackets;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Execution, Pipe, NumPackets, PacketSize, PacketAlignment }.Concat(ExtraIDs);

        public OpGroupReserveReadPipePackets() {}

        private OpGroupReserveReadPipePackets(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Execution = new ID(codes[i++]);
            Pipe = new ID(codes[i++]);
            NumPackets = new ID(codes[i++]);
            PacketSize = new ID(codes[i++]);
            PacketAlignment = new ID(codes[i++]);
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
            codes[i++] = mapID(Pipe);
            codes[i++] = mapID(NumPackets);
            codes[i++] = mapID(PacketSize);
            codes[i++] = mapID(PacketAlignment);
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

