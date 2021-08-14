// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpVariable : MemoryInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public StorageClass StorageClass { get; init; }
        public ID? Initializer { get; init; }

        public override OpCode OpCode => OpCode.OpVariable;
        public override int WordCount => 1 + 1 + 1 + 1 + (Initializer.HasValue ? 1 : 0);
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(new[] { Initializer }
                    .Where(o => o.HasValue)
                    .Select(o => o!.Value));
                return result;
            }
        }

        public OpVariable() {}

        private OpVariable(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            StorageClass = (StorageClass)codes[i++];
            if (i < end)
                Initializer = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = (uint)StorageClass;
            if (Initializer.HasValue)
            {
                codes[i++] = mapID(Initializer.Value);
            }
        }
    }
}

