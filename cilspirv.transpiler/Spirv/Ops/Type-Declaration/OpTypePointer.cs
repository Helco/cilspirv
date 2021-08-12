// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypePointer : TypeDeclarationInstruction
    {
        public ID Result1 { get; init; }
        public StorageClass StorageClass2 { get; init; }
        public ID Type { get; init; }

        public override OpCode OpCode => OpCode.OpTypePointer;
        public override int WordCount => 1 + 1 + 1 + 1;
        public override ID? ResultID => Result1;

        public override IEnumerable<ID> AllIDs => new[] { Result1, Type };

        public OpTypePointer() {}

        private OpTypePointer(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result1 = new ID(codes[i++]);
            StorageClass2 = (StorageClass)codes[i++];
            Type = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result1.Value;
            codes[i++] = (uint)StorageClass2;
            codes[i++] = Type.Value;
        }
    }
}

