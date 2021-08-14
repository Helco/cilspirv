// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Pipes })]
    public sealed record OpReservedWritePipe : PipeInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Pipe { get; init; }
        public ID ReserveId { get; init; }
        public ID Index { get; init; }
        public ID Pointer { get; init; }
        public ID PacketSize { get; init; }
        public ID PacketAlignment { get; init; }

        public override OpCode OpCode => OpCode.OpReservedWritePipe;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Pipe, ReserveId, Index, Pointer, PacketSize, PacketAlignment };

        public OpReservedWritePipe() {}

        private OpReservedWritePipe(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Pipe = new ID(codes[i++]);
            ReserveId = new ID(codes[i++]);
            Index = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            PacketSize = new ID(codes[i++]);
            PacketAlignment = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Pipe);
            codes[i++] = mapID(ReserveId);
            codes[i++] = mapID(Index);
            codes[i++] = mapID(Pointer);
            codes[i++] = mapID(PacketSize);
            codes[i++] = mapID(PacketAlignment);
        }
    }
}
