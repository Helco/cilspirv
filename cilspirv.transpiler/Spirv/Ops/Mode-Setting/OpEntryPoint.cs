// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpEntryPoint : ModeSettingInstruction
    {
        public ExecutionModel ExecutionModel { get; init; }
        public ID EntryPoint { get; init; }
        public LiteralString Name { get; init; }
        public ImmutableArray<ID> Interface { get; init; }

        public override OpCode OpCode => OpCode.OpEntryPoint;
        public override int WordCount => 1 + 1 + 1 + Name.WordCount + Interface.Length + ExtraWordCount;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { EntryPoint });
                result = result.Concat(Interface);
                return result;
            }
        }

        public OpEntryPoint() {}

        private OpEntryPoint(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ExecutionModel = (ExecutionModel)codes[i++];
            EntryPoint = new ID(codes[i++]);
            Name = new LiteralString(codes, ref i);
            Interface = codes.Skip(i).Take(end - i)
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
            codes[i++] = (uint)ExecutionModel;
            codes[i++] = mapID(EntryPoint);
            Name.Write(codes, ref i);
            foreach (var x in Interface)
            {
                codes[i++] = mapID(x);
            }
            foreach (var o in ExtraOperands)
                o.Write(codes, ref i, mapID);
        }
    }
}

