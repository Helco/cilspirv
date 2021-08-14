// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpExtInst : ExtensionInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Set { get; init; }
        public uint Instruction { get; init; }
        public ImmutableArray<ID> Operands { get; init; }

        public override OpCode OpCode => OpCode.OpExtInst;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + Operands.Length + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { ResultType, Result, Set });
                result = result.Concat(Operands);
                return result;
            }
        }

        public OpExtInst() {}

        private OpExtInst(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Set = new ID(codes[i++]);
            Instruction = codes[i++];
            Operands = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
                .ToImmutableArray();
            i = end;
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
            codes[i++] = mapID(Set);
            codes[i++] = (uint)Instruction;
            foreach (var x in Operands)
            {
                codes[i++] = mapID(x);
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

