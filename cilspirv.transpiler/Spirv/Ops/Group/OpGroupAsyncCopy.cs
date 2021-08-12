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
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Execution { get; init; }
        public ID Destination { get; init; }
        public ID Source { get; init; }
        public ID NumElements { get; init; }
        public ID Stride { get; init; }
        public ID Event { get; init; }

        public override OpCode OpCode => OpCode.OpGroupAsyncCopy;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Execution, Destination, Source, NumElements, Stride, Event };

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
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = Execution.Value;
            codes[i++] = Destination.Value;
            codes[i++] = Source.Value;
            codes[i++] = NumElements.Value;
            codes[i++] = Stride.Value;
            codes[i++] = Event.Value;
        }
    }
}

