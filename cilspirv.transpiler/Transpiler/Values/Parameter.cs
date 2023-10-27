using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler.Values
{
    internal class Parameter :
        BaseValueBehaviour,
        IDecoratableInstructionGeneratable,
        IDebugInstructionGeneratable
    {
        public int SpirvIndex { get; } // this can be different from the CLI arg index
        public string Name { get; }
        public SpirvType Type { get; }
        public IReadOnlySetDecorationEntry Decorations { get; set; } = new HashSet<DecorationEntry>();

        public Parameter(int index, string name, SpirvType type) => (SpirvIndex, Name, Type) = (index, name, type);

        IEnumerable<Instruction> IDecoratableInstructionGeneratable.GenerateDecorations(IIDMapper mapper) => this.BaseGenerateDecorations(mapper);

        public IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpFunctionParameter()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(Type)
            };
        }

        public IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context)
        {
            yield return new OpName()
            {
                Target = context.IDOf(this),
                Name = Name
            };
        }

        public override IEnumerable<Instruction>? Load(ITranspilerValueContext context)
        {
            context.Result = new ValueStackEntry(this, context.IDOf(this), Type);
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction>? Store(ITranspilerValueContext context, ValueStackEntry value) =>
            throw new InvalidOperationException($"Cannot store values into parameters");
    }
}