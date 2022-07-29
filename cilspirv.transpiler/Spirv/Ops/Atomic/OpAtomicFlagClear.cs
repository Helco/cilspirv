// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Kernel })]
    public sealed record OpAtomicFlagClear : AtomicInstruction
    {
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Semantics { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicFlagClear;
        public override int WordCount => 1 + 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { Pointer, Memory, Semantics }.Concat(ExtraIDs);

        public OpAtomicFlagClear() {}

        private OpAtomicFlagClear(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Semantics = new ID(codes[i++]);
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
            codes[i++] = mapID(Pointer);
            codes[i++] = mapID(Memory);
            codes[i++] = mapID(Semantics);
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(Pointer);
            writer.Write(' ');
            writer.Write(Memory);
            writer.Write(' ');
            writer.Write(Semantics);
        }
    }
}

