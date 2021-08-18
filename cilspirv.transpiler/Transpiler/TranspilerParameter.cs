using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerParameter : IDecoratableInstructionGeneratable, IDebugInstructionGeneratable, IMappedFromCILParam
    {
        public int SpirvIndex { get; } // this can be different from the CLI arg index
        public string Name { get; }
        public SpirvType Type { get; }
        public IReadOnlySet<DecorationEntry> Decorations { get; set; } = new HashSet<DecorationEntry>();

        public TranspilerParameter(int index, string name, SpirvType type) => (SpirvIndex, Name, Type) = (index, name, type);

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpFunctionParameter()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(Type)
            };
        }

        public IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context)
        {
            yield return new OpName()
            {
                Target = context.IDOf(this),
                Name = Name
            };
        }
    }
}