// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSelectionMerge : ControlFlowInstruction
    {
        public ID MergeBlock { get; init; }
        public SelectionControl SelectionControl { get; init; }

        public override OpCode OpCode => OpCode.OpSelectionMerge;
        public override int WordCount => 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { MergeBlock }.Concat(ExtraIDs);

        public OpSelectionMerge() {}

        private OpSelectionMerge(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            MergeBlock = new ID(codes[i++]);
            SelectionControl = (SelectionControl)codes[i++];
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
            codes[i++] = mapID(MergeBlock);
            codes[i++] = (uint)SelectionControl;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(MergeBlock);
            writer.Write(' ');
            writer.Write(SelectionControl);
        }
    }
}

