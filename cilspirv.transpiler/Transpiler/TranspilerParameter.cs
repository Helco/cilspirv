using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerParameter : IDecoratableInstructionGeneratable
    {
        public string Name { get; }
        public SpirvType Type { get; }
        public ISet<DecorationEntry> Decorations { get; } = new HashSet<DecorationEntry>();

        public TranspilerParameter(string name, SpirvType type) => (Name, Type) = (name, type);

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpFunctionParameter()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(Type)
            };
        }
    }
}