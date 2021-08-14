using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace cilspirv.Spirv.Ops
{
    /// <summary>
    /// Default class for unrecognized instructions
    /// </summary>
    public sealed record OpUnknown : Instruction
    {
        public readonly ImmutableArray<uint> Args;

        public override OpCode OpCode { get; }

        public override int WordCount => Args.Length + 1;

        public OpUnknown(OpCode opCode, IEnumerable<uint> args)
        {
            OpCode = opCode;
            Args = args.ToImmutableArray();
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            codes[0] = InstructionCode;
            Args.AsSpan().CopyTo(codes.Slice(1));
        }
    }
}
