// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.VectorComputeINTEL })]
    public sealed record OpTypeBufferSurfaceINTEL : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public AccessQualifier AccessQualifier { get; init; }

        public override OpCode OpCode => OpCode.OpTypeBufferSurfaceINTEL;
        public override int WordCount => 1 + 1 + 1;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result };

        public OpTypeBufferSurfaceINTEL() {}

        private OpTypeBufferSurfaceINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            AccessQualifier = (AccessQualifier)codes[i++];
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = Result.Value;
            codes[i++] = (uint)AccessQualifier;
        }
    }
}

