using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace cilspirv.Spirv
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

        protected override void WriteCode(Span<uint> code) => 
            Args.AsSpan().CopyTo(code);
    }
}
