// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypePointer : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public StorageClass StorageClass { get; init; }
        public ID Type { get; init; }

        public override OpCode OpCode => OpCode.OpTypePointer;
        public override int WordCount => 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result, Type };

        public OpTypePointer() {}

        private OpTypePointer(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            StorageClass = (StorageClass)codes[i++];
            Type = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result.Value;
            codes[i++] = (uint)StorageClass;
            codes[i++] = Type.Value;
        }
    }
}

