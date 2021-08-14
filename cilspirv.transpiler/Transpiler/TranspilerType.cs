using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class TranspilerType : IDecoratableInstructionGeneratable
    {
        public string? Name { get; }
        public virtual SpirvType Type { get; }
        public ISet<DecorationEntry> Decorations { get; } = new HashSet<DecorationEntry>();

        public TranspilerType(string? name, SpirvType type) => (Name, Type) = (name, type);

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context) => Type.GenerateInstructions(context);
    }
}
