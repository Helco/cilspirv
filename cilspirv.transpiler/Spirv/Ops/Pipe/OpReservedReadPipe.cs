// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Pipes })]
    public sealed record OpReservedReadPipe : PipeInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Pipe { get; init; }
        public ID ReserveId { get; init; }
        public ID Index { get; init; }
        public ID Pointer { get; init; }
        public ID PacketSize { get; init; }
        public ID PacketAlignment { get; init; }

        public override OpCode OpCode => OpCode.OpReservedReadPipe;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Pipe, ReserveId, Index, Pointer, PacketSize, PacketAlignment };

        public OpReservedReadPipe() {}

        private OpReservedReadPipe(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Pipe = new ID(codes[i++]);
            ReserveId = new ID(codes[i++]);
            Index = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            PacketSize = new ID(codes[i++]);
            PacketAlignment = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Pipe.Value;
            codes[i++] = ReserveId.Value;
            codes[i++] = Index.Value;
            codes[i++] = Pointer.Value;
            codes[i++] = PacketSize.Value;
            codes[i++] = PacketAlignment.Value;
        }
    }
}

