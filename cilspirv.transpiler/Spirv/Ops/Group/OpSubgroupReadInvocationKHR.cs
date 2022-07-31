// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.SubgroupBallotKHR }, Extensions = new[] { "SPV_KHR_shader_ballot" })]
    public sealed record OpSubgroupReadInvocationKHR : GroupInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Value { get; init; }
        public ID Index { get; init; }

        public override OpCode OpCode => OpCode.OpSubgroupReadInvocationKHR;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Value, Index }.Concat(ExtraIDs);

        public OpSubgroupReadInvocationKHR() {}

        private OpSubgroupReadInvocationKHR(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Value = new ID(codes[i++]);
            Index = new ID(codes[i++]);
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
            codes[i++] = mapID(Value);
            codes[i++] = mapID(Index);
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
            writer.Write(Value);
            writer.Write(' ');
            writer.Write(Index);
            DisassembleExtras(writer);
        }
    }
}

