// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Pipes })]
    public sealed record OpWritePipe : PipeInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Pipe { get; init; }
        public ID Pointer { get; init; }
        public ID PacketSize { get; init; }
        public ID PacketAlignment { get; init; }

        public override OpCode OpCode => OpCode.OpWritePipe;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Pipe, Pointer, PacketSize, PacketAlignment };

        public OpWritePipe() {}

        private OpWritePipe(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Pipe = new ID(codes[i++]);
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
            codes[i++] = mapID(Pointer);
            codes[i++] = mapID(PacketSize);
            codes[i++] = mapID(PacketAlignment);
        }
    }
}

