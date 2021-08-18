using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerVariable :
        IDecoratableInstructionGeneratable,
        IDebugInstructionGeneratable,
        IMappedFromCILField,
        IMappedFromCILParam
    {
        public string Name { get; }
        public SpirvPointerType PointerType { get; }
        public SpirvType ElementType => PointerType.Type ?? throw new InvalidOperationException("Element type is not set");
        public StorageClass StorageClass => PointerType.StorageClass;
        public IReadOnlySet<DecorationEntry> Decorations { get; set; } = new HashSet<DecorationEntry>();

        public TranspilerVariable(string name, SpirvPointerType pointerType) =>
            (Name, PointerType) = (name, pointerType);

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpVariable()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(PointerType),
                StorageClass = StorageClass
            };
        }

        public IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context)
        {
            if (!string.IsNullOrEmpty(Name))
                yield return new OpName()
                {
                    Target = context.IDOf(this),
                    Name = Name
                };
        }
    }
}