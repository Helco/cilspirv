// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.4", Extensions = new[] { "SPV_GOOGLE_decorate_string", "SPV_GOOGLE_hlsl_functionality1" })]
    public sealed record OpDecorateString : AnnotationInstruction
    {
        public ID Target { get; init; }
        public Decoration Decoration { get; init; }

        public override OpCode OpCode => OpCode.OpDecorateString;
        public override int WordCount => 1 + 1 + 1;

        public override IEnumerable<ID> AllIDs => new[] { Target };

        public OpDecorateString() {}

        private OpDecorateString(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Target = new ID(codes[i++]);
            Decoration = (Decoration)codes[i++];
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Target);
            codes[i++] = (uint)Decoration;
        }
    }
}

