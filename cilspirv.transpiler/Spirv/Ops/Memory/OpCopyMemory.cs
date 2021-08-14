// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpCopyMemory : MemoryInstruction
    {
        public ID Target { get; init; }
        public ID Source { get; init; }
        public MemoryAccess? MemoryAccess1 { get; init; }
        public MemoryAccess? MemoryAccess2 { get; init; }

        public override OpCode OpCode => OpCode.OpCopyMemory;
        public override int WordCount => 1 + 1 + 1 + (MemoryAccess1.HasValue ? 1 : 0) + (MemoryAccess2.HasValue ? 1 : 0) + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { Target, Source }.Concat(ExtraIDs);

        public OpCopyMemory() {}

        private OpCopyMemory(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Target = new ID(codes[i++]);
            Source = new ID(codes[i++]);
            if (i < end)
                MemoryAccess1 = (MemoryAccess)codes[i++];
            if (i < end)
                MemoryAccess2 = (MemoryAccess)codes[i++];
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
            codes[i++] = mapID(Target);
            codes[i++] = mapID(Source);
            if (MemoryAccess1.HasValue)
            {
                codes[i++] = (uint)MemoryAccess1.Value;
            }
            if (MemoryAccess2.HasValue)
            {
                codes[i++] = (uint)MemoryAccess2.Value;
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

