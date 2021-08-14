using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerFunction : IDecoratableInstructionGeneratable
    {
        public string Name { get; }
        public TranspilerType ReturnType { get; set; }
        public IList<TranspilerParameter> Parameters { get; } = new List<TranspilerParameter>();
        public ISet<DecorationEntry> Decorations { get; } = new HashSet<DecorationEntry>();
        public FunctionControl FunctionControl { get; set; }

        public SpirvFunctionType SpirvFunctionType => new SpirvFunctionType()
        {
            ReturnType = ReturnType.Type,
            ParameterTypes = Parameters.Select(p => p.Type).ToImmutableArray()
        };

        public TranspilerFunction(string name, TranspilerType returnType) =>
            (Name, ReturnType) = (name, returnType);

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context) =>
            Parameters.SelectMany(p => p.GenerateInstructions(context))
            .Prepend(new OpFunction()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(ReturnType.Type),
                FunctionType = context.IDOf(SpirvFunctionType),
                FunctionControl = FunctionControl
            }).Concat(GenerateBody(context))
            .Append(new OpFunctionEnd())
            .GetEnumerator();           

        protected virtual IEnumerable<Instruction> GenerateBody(IInstructionGeneratorContext context) => Enumerable.Empty<Instruction>();
    }

    internal class TranspilerDefinedFunction : TranspilerFunction
    {
        public IList<TranspilerBlock> Blocks { get; } = new List<TranspilerBlock>();

        public TranspilerDefinedFunction(string name, TranspilerType returnType) : base(name, returnType) { }

        protected override IEnumerable<Instruction> GenerateBody(IInstructionGeneratorContext context) =>
            Blocks.SelectMany(b => b.GenerateInstructions(context));
    }

    internal class TranspilerEntryFunction : TranspilerFunction
    {
        public ExecutionModel ExecutionModel { get; }
        public ISet<TranspilerVariable> Interface { get; } = new HashSet<TranspilerVariable>();

        public TranspilerEntryFunction(string name, TranspilerType returnType, ExecutionModel model) : base(name, returnType)
            => ExecutionModel = model;

        public Instruction GenerateEntryPoint(IInstructionGeneratorContext context) => new OpEntryPoint()
        {
            ExecutionModel = ExecutionModel,
            EntryPoint = context.IDOf(this),
            Name = Name,
            Interface = Interface.Select(context.IDOf).ToImmutableArray()
        };
    }
}