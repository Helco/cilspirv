// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.ExpectAssumeKHR }, Extensions = new[] { "SPV_KHR_expect_assume" })]
    public sealed record OpExpectKHR : MiscellaneousInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Value { get; init; }
        public ID ExpectedValue { get; init; }

        public override OpCode OpCode => OpCode.OpExpectKHR;
        public override int WordCount => 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs => new[] { ResultType, Result, Value, ExpectedValue };

        public OpExpectKHR() {}

        private OpExpectKHR(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Value = new ID(codes[i++]);
            ExpectedValue = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType.Value;
            codes[i++] = Result.Value;
            codes[i++] = Value.Value;
            codes[i++] = ExpectedValue.Value;
        }
    }
}

