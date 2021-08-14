using System;
using System.Collections.Generic;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerType : IDecoratableInstructionGeneratable, IDebugInstructionGeneratable
    {
        public string? Name { get; }
        public virtual SpirvType Type { get; }
        public ISet<DecorationEntry> Decorations { get; } = new HashSet<DecorationEntry>();

        public TranspilerType(string? name, SpirvType type) => (Name, Type) = (name, type);

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context) => Type.GenerateInstructions(context);

        public virtual IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context)
        {
            if (Name != null)
                yield return new OpName()
                {
                    Target = context.IDOf(this),
                    Name = Name
                };
        }
    }
}
