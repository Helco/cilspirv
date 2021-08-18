using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class TranspilerVarGroup : IMappedFromCILType, IMappedFromCILField, IMappedFromCILParam
    {
        public TypeDefinition TypeDefinition { get; }
        public string Name { get; }
        public List<TranspilerVariable> Variables { get; } = new List<TranspilerVariable>();

        public TranspilerVarGroup(string name, TypeDefinition typeDef) =>
            (Name, TypeDefinition) = (name, typeDef);
    }
}
