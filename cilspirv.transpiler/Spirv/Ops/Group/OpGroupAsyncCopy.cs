// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Kernel })]
    public sealed record OpGroupAsyncCopy : GroupInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Execution { get; init; }
        public ID Destination { get; init; }
        public ID Source { get; init; }
        public ID NumElements { get; init; }
        public ID Stride { get; init; }
        public ID Event { get; init; }

        public override OpCode OpCode => OpCode.OpGroupAsyncCopy;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Execution, Destination, Source, NumElements, Stride, Event };

        public OpGroupAsyncCopy() {}

        private OpGroupAsyncCopy(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Execution = new ID(codes[i++]);
            Destination = new ID(codes[i++]);
            Source = new ID(codes[i++]);
            NumElements = new ID(codes[i++]);
            Stride = new ID(codes[i++]);
            Event = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Execution.Value;
            codes[i++] = Destination.Value;
            codes[i++] = Source.Value;
            codes[i++] = NumElements.Value;
            codes[i++] = Stride.Value;
            codes[i++] = Event.Value;
        }
    }
}

