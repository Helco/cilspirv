// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpExtInst : ExtensionInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Set { get; init; }
        public uint Instruction { get; init; }
        public ImmutableArray<ID> Operands { get; init; }

        public override OpCode OpCode => OpCode.OpExtInst;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + Operands.Length;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
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
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Set = new ID(codes[i++]);
            Instruction = codes[i++];
            Operands = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Set.Value;
            codes[i++] = (uint)Instruction;
            foreach (var x in Operands)
            {
                codes[i++] = x.Value;
            }
        }
    }
}

