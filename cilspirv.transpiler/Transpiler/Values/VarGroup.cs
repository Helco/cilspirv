using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;

namespace cilspirv.Transpiler.Values
{
    internal class VarGroup :
        IMappedFromCILType,
        IValueBehaviour
    {
        public TypeDefinition TypeDefinition { get; }
        public string Name { get; }
        public List<Variable> Variables { get; } = new List<Variable>();

        public VarGroup(string name, TypeDefinition typeDef) =>
            (Name, TypeDefinition) = (name, typeDef);

        IEnumerable<Instruction> IValueBehaviour.LoadAddress(ITranspilerValueContext context)
        {
            context.Result = new StackEntry(this);
            return Enumerable.Empty<Instruction>();
        }

        IEnumerable<Instruction> IValueBehaviour.Load(ITranspilerValueContext context)
        {
            context.Result = new StackEntry(this);
            return Enumerable.Empty<Instruction>();
        }

        IEnumerable<Instruction> IValueBehaviour.Store(ITranspilerValueContext context, ValueStackEntry value) =>
            throw new InvalidOperationException("Cannot store a variable group");
    }
}

namespace cilspirv.Transpiler
{
    // a variable group with missing storage class
    internal sealed record TranspilerVarGroupTemplate : IMappedFromCILType
    {
        public TypeDefinition TypeDefinition { get; }
        public string Name => TypeDefinition.Name;

        public TranspilerVarGroupTemplate(TypeDefinition typeDef) =>
            TypeDefinition = typeDef;
    }
}
