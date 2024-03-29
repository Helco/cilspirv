// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Kernel })]
    public sealed record OpGroupAsyncCopy : GroupInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Execution { get; init; }
        public ID Destination { get; init; }
        public ID Source { get; init; }
        public ID NumElements { get; init; }
        public ID Stride { get; init; }
        public ID Event { get; init; }

        public override OpCode OpCode => OpCode.OpGroupAsyncCopy;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Execution, Destination, Source, NumElements, Stride, Event }.Concat(ExtraIDs);

        public OpGroupAsyncCopy() {}

        private OpGroupAsyncCopy(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Execution = new ID(codes[i++]);
            Destination = new ID(codes[i++]);
            Source = new ID(codes[i++]);
            NumElements = new ID(codes[i++]);
            Stride = new ID(codes[i++]);
            Event = new ID(codes[i++]);
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
            codes[i++] = mapID(Execution);
            codes[i++] = mapID(Destination);
            codes[i++] = mapID(Source);
            codes[i++] = mapID(NumElements);
            codes[i++] = mapID(Stride);
            codes[i++] = mapID(Event);
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(ResultType);
            writer.Write(' ');
            writer.Write(Execution);
            writer.Write(' ');
            writer.Write(Destination);
            writer.Write(' ');
            writer.Write(Source);
            writer.Write(' ');
            writer.Write(NumElements);
            writer.Write(' ');
            writer.Write(Stride);
            writer.Write(' ');
            writer.Write(Event);
            DisassembleExtras(writer);
        }
    }
}

