using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;

namespace cilspirv.Transpiler.Values
{
    internal class VarGroup :
        BaseValueBehaviour,
        IMappedFromCILType
    {
        public TypeDefinition TypeDefinition { get; }
        public string Name { get; }
        public List<Variable> Variables { get; } = new List<Variable>();

        public VarGroup(string name, TypeDefinition typeDef) =>
            (Name, TypeDefinition) = (name, typeDef);

        public override IEnumerable<Instruction> LoadAddress(ITranspilerValueContext context)
        {
            context.Result = new StackEntry(this);
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> Load(ITranspilerValueContext context)
        {
            context.Result = new StackEntry(this);
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> Store(ITranspilerValueContext context, ValueStackEntry value) =>
            throw new InvalidOperationException("Cannot store a variable group");
    }

    // a variable group with missing storage class
    internal sealed record VarGroupTemplate : IMappedFromCILType
    {
        public TypeDefinition TypeDefinition { get; }
        public string Name => TypeDefinition.Name;

        public VarGroupTemplate(TypeDefinition typeDef) =>
            TypeDefinition = typeDef;
    }
}
