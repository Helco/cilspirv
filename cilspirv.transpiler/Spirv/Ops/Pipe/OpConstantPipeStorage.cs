// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.1", Capabilities = new[] { Capability.PipeStorage })]
    public sealed record OpConstantPipeStorage : PipeInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public LiteralNumber PacketSize { get; init; }
        public LiteralNumber PacketAlignment { get; init; }
        public LiteralNumber Capacity { get; init; }

        public override OpCode OpCode => OpCode.OpConstantPipeStorage;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2 };

        public OpConstantPipeStorage() {}

        private OpConstantPipeStorage(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            PacketSize = (LiteralNumber)codes[i++];
            PacketAlignment = (LiteralNumber)codes[i++];
            Capacity = (LiteralNumber)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = PacketSize.Value;
            codes[i++] = PacketAlignment.Value;
            codes[i++] = Capacity.Value;
        }
    }
}

