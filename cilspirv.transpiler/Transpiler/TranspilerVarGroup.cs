using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class TranspilerVarGroup :
        IMappedFromCILType,
        IMappedFromCILField,
        IMappedFromCILParam,
        ITranspilerFieldBehavior
    {
        public TypeDefinition TypeDefinition { get; }
        public string Name { get; }
        public List<TranspilerVariable> Variables { get; } = new List<TranspilerVariable>();

        public TranspilerVarGroup(string name, TypeDefinition typeDef) =>
            (Name, TypeDefinition) = (name, typeDef);

        IEnumerable<Instruction> ITranspilerFieldBehavior.LoadAddress(ITranspilerFieldContext context)
        {
            context.Result = new StackEntry(this);
            return Enumerable.Empty<Instruction>();
        }

        IEnumerable<Instruction> ITranspilerFieldBehavior.Load(ITranspilerFieldContext context) =>
            throw new InvalidOperationException("Cannot load a variable group");

        IEnumerable<Instruction> ITranspilerFieldBehavior.Store(ITranspilerFieldContext context, ValueStackEntry value) =>
            throw new InvalidOperationException("Cannot load a variable group");
    }

    // a variable group with missing storage class
    internal sealed record TranspilerVarGroupTemplate : IMappedFromCILType
    {
        public TypeDefinition TypeDefinition { get; }
        public string Name => TypeDefinition.Name;

        public TranspilerVarGroupTemplate(TypeDefinition typeDef) =>
            (TypeDefinition) = (typeDef);
    }
}
