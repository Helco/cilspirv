using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerType : IDecoratableInstructionGeneratable, IDebugInstructionGeneratable, IWrapperInstructionGeneratable
    {
        public string? Name { get; }
        public virtual SpirvType Type { get; }
        public ISet<DecorationEntry> Decorations { get; } = new HashSet<DecorationEntry>();

        public TranspilerType(string? name, SpirvType type) => (Name, Type) = (name, type);

        public IInstructionGeneratable WrappedGeneratable => Type;
        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context) => Enumerable.Empty<Instruction>().GetEnumerator();

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
