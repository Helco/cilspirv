using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class TranspilerFunction
    {
        public string Name { get; }
        public SpirvFunctionType FunctionType { get; }
        public IList<string> ParameterNames { get; } = new List<string>();
        public IList<TranspilerBlock> Blocks { get; } = new List<TranspilerBlock>();

        public TranspilerFunction(string name, SpirvFunctionType type) =>
            (Name, FunctionType) = (name, type);
    }

    internal class TranspilerEntryFunction : TranspilerFunction
    {
        public ExecutionModel ExecutionModel { get; }
        public ISet<TranspilerVariable> Interface { get; } = new HashSet<TranspilerVariable>();

        public TranspilerEntryFunction(string name, SpirvFunctionType type, ExecutionModel model) : base(name, type)
            => ExecutionModel = model;
    }
}