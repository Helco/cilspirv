// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.1")]
    public sealed record OpModuleProcessed : DebugInstruction
    {
        public LiteralString Process { get; init; }

        public override OpCode OpCode => OpCode.OpModuleProcessed;
        public override int WordCount => 1 + Process.WordCount;


        public OpModuleProcessed() {}

        private OpModuleProcessed(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Process = new LiteralString(codes, ref i);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            Process.Write(codes, ref i);
        }
    }
}

