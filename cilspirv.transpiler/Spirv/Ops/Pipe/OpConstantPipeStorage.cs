// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.1", Capabilities = new[] { Capability.PipeStorage })]
    public sealed record OpConstantPipeStorage : PipeInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public LiteralNumber PacketSize { get; init; }
        public LiteralNumber PacketAlignment { get; init; }
        public LiteralNumber Capacity { get; init; }

        public override OpCode OpCode => OpCode.OpConstantPipeStorage;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result }.Concat(ExtraIDs);

        public OpConstantPipeStorage() {}

        private OpConstantPipeStorage(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            PacketSize = (LiteralNumber)codes[i++];
            PacketAlignment = (LiteralNumber)codes[i++];
            Capacity = (LiteralNumber)codes[i++];
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
            codes[i++] = PacketSize.Value;
            codes[i++] = PacketAlignment.Value;
            codes[i++] = Capacity.Value;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(ResultType);
            writer.Write(' ');
            writer.Write(PacketSize);
            writer.Write(' ');
            writer.Write(PacketAlignment);
            writer.Write(' ');
            writer.Write(Capacity);
            DisassembleExtras(writer);
        }
    }
}

