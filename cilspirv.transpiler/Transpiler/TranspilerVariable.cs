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
        IMappedFromCILParam,
        ITranspilerFieldBehavior
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

        public void MarkUsageIn(TranspilerFunction function)
        {
            if (function is not TranspilerEntryFunction entryFunction)
                return;
            if (StorageClass == StorageClass.Input || StorageClass == StorageClass.Output)
                entryFunction.Interface.Add(this);
        }

        IEnumerable<Instruction> ITranspilerFieldBehavior.Load(ITranspilerFieldContext context)
        {
            MarkUsageIn(context.Function);
            var result = new ValueStackEntry(this, context.CreateID(), ElementType);
            context.Result = result;
            yield return new OpLoad()
            {
                Result = result.ID,
                ResultType = context.IDOf(ElementType),
                Pointer = context.IDOf(this)
            };
        }

        IEnumerable<Instruction> ITranspilerFieldBehavior.LoadAddress(ITranspilerFieldContext context)
        {
            MarkUsageIn(context.Function);
            context.Result = new ValueStackEntry(this, context.IDOf(this), PointerType);
            return Enumerable.Empty<Instruction>();
        }

        IEnumerable<Instruction> ITranspilerFieldBehavior.Store(ITranspilerFieldContext context, ValueStackEntry value)
        {
            MarkUsageIn(context.Function);
            yield return new OpStore()
            {
                Pointer = context.IDOf(this),
                Object = value.ID
            };
        }
    }
}