// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Pipes })]
    public sealed record OpCommitReadPipe : PipeInstruction
    {
        public ID Pipe { get; init; }
        public ID ReserveId { get; init; }
        public ID PacketSize { get; init; }
        public ID PacketAlignment { get; init; }

        public override OpCode OpCode => OpCode.OpCommitReadPipe;
        public override int WordCount => 1 + 1 + 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Pipe, ReserveId, PacketSize, PacketAlignment };

        public OpCommitReadPipe() {}

        private OpCommitReadPipe(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Pipe = new ID(codes[i++]);
            ReserveId = new ID(codes[i++]);
            PacketSize = new ID(codes[i++]);
            PacketAlignment = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Pipe.Value;
            codes[i++] = ReserveId.Value;
            codes[i++] = PacketSize.Value;
            codes[i++] = PacketAlignment.Value;
        }
    }
}

