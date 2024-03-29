// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Pipes })]
    public sealed record OpGroupCommitReadPipe : PipeInstruction
    {
        public ID Execution { get; init; }
        public ID Pipe { get; init; }
        public ID ReserveId { get; init; }
        public ID PacketSize { get; init; }
        public ID PacketAlignment { get; init; }

        public override OpCode OpCode => OpCode.OpGroupCommitReadPipe;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { Execution, Pipe, ReserveId, PacketSize, PacketAlignment }.Concat(ExtraIDs);

        public OpGroupCommitReadPipe() {}

        private OpGroupCommitReadPipe(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Execution = new ID(codes[i++]);
            Pipe = new ID(codes[i++]);
            ReserveId = new ID(codes[i++]);
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
            codes[i++] = mapID(Execution);
            codes[i++] = mapID(Pipe);
            codes[i++] = mapID(ReserveId);
            codes[i++] = mapID(PacketSize);
            codes[i++] = mapID(PacketAlignment);
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(Execution);
            writer.Write(' ');
            writer.Write(Pipe);
            writer.Write(' ');
            writer.Write(ReserveId);
            writer.Write(' ');
            writer.Write(PacketSize);
            writer.Write(' ');
            writer.Write(PacketAlignment);
            DisassembleExtras(writer);
        }
    }
}

