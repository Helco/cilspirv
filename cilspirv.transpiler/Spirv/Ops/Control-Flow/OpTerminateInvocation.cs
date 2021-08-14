// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.Shader }, Extensions = new[] { "SPV_KHR_terminate_invocation" })]
    public sealed record OpTerminateInvocation : ControlFlowInstruction
    {

        public override OpCode OpCode => OpCode.OpTerminateInvocation;
        public override int WordCount => 1;


        public OpTerminateInvocation() {}

        private OpTerminateInvocation(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
        }
    }
}
