using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler.Values
{
    internal class LocalVariable : Variable
    {
        public LocalVariable(string name, SpirvPointerType pointerType) : base(name, pointerType)
        {
        }
    }

    internal class GlobalVariable : Variable
    {
        public GlobalVariable(string name, SpirvPointerType pointerType) : base(name, pointerType)
        {
        }
    }

    internal abstract class Variable :
        BaseValueBehaviour,
        IDecoratableInstructionGeneratable,
        IDebugInstructionGeneratable
    {
        public string Name { get; }
        public SpirvPointerType PointerType { get; }
        public SpirvType ElementType => PointerType.Type ?? throw new InvalidOperationException("Element type is not set");
        public StorageClass StorageClass => PointerType.StorageClass;
        public IReadOnlySetDecorationEntry Decorations { get; set; } = new HashSet<DecorationEntry>();

        public Variable(string name, SpirvPointerType pointerType) =>
            (Name, PointerType) = (name, pointerType);

        IEnumerable<Instruction> IDecoratableInstructionGeneratable.GenerateDecorations(IIDMapper mapper) => this.BaseGenerateDecorations(mapper);

        public virtual IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpVariable()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(PointerType),
                StorageClass = StorageClass
            };
        }

        public virtual IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context)
        {
            if (!string.IsNullOrEmpty(Name))
                yield return new OpName()
                {
                    Target = context.IDOf(this),
                    Name = Name
                };
        }

        public void MarkUsageIn(Function function)
        {
            if (function is not EntryFunction entryFunction)
                return;
            if (StorageClass == StorageClass.Input || StorageClass == StorageClass.Output)
                entryFunction.Interface.Add(this);
        }

        public override IEnumerable<Instruction> Load(ITranspilerValueContext context)
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

        public override IEnumerable<Instruction> LoadAddress(ITranspilerValueContext context)
        {
            MarkUsageIn(context.Function);
            context.Result = new ValueStackEntry(this, context.IDOf(this), PointerType);
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> Store(ITranspilerValueContext context, ValueStackEntry value)
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