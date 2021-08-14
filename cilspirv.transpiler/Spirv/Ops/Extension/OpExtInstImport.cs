// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpExtInstImport : ExtensionInstruction
    {
        public ID Result { get; init; }
        public LiteralString Name { get; init; }

        public override OpCode OpCode => OpCode.OpExtInstImport;
        public override int WordCount => 1 + 1 + Name.WordCount;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result };

        public OpExtInstImport() {}

        private OpExtInstImport(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            Name = new LiteralString(codes, ref i);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Result);
            Name.Write(codes, ref i);
        }
    }
}

