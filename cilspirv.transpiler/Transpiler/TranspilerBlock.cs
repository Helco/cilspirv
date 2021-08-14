using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class TranspilerBlock
    {
        public TranspilerFunction Parent { get; }
        public IList<Instruction> Instructions { get; } = new List<Instruction>();

        public TranspilerBlock(TranspilerFunction parent) => Parent = parent;
    }
}