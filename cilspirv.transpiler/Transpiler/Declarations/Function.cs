using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Values;

namespace cilspirv.Transpiler.Declarations
{
    internal class Function : IDecoratableInstructionGeneratable, IDebugInstructionGeneratable
    {
        public string Name { get; }
        public SpirvType ReturnType { get; }
        public IValueBehaviour? ReturnValue { get; set; }
        public IList<Parameter> Parameters { get; } = new List<Parameter>();
        public IReadOnlySet<DecorationEntry> Decorations { get; set; } = new HashSet<DecorationEntry>();
        public FunctionControl FunctionControl { get; set; }

        public SpirvFunctionType SpirvFunctionType => new SpirvFunctionType()
        {
            ReturnType = ReturnType,
            ParameterTypes = Parameters.Select(p => p.Type).ToImmutableArray()
        };

        public Function(string name, SpirvType returnType) =>
            (Name, ReturnType) = (name, returnType);

        public IEnumerator<Instruction> GenerateInstructions(IIDMapper context) =>
            Parameters.SelectMany(p => p.GenerateInstructions(context))
            .Prepend(new OpFunction()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(ReturnType),
                FunctionType = context.IDOf(SpirvFunctionType),
                FunctionControl = FunctionControl
            }).Concat(GenerateBody(context))
            .Append(new OpFunctionEnd())
            .GetEnumerator();

        protected virtual IEnumerable<Instruction> GenerateBody(IIDMapper context) => Enumerable.Empty<Instruction>();

        public IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context)
        {
            yield return new OpName()
            {
                Target = context.IDOf(this),
                Name = Name
            };
        }
    }

    internal class DefinedFunction : Function
    {
        public IList<Block> Blocks { get; } = new List<Block>();
        public IList<Variable> Variables { get; } = new List<Variable>();

        public DefinedFunction(string name, SpirvType returnType) : base(name, returnType) { }

        protected override IEnumerable<Instruction> GenerateBody(IIDMapper context)
        {
            var instructions = Blocks.SelectMany(b => b.GenerateInstructions(context));
            if (Variables.Any())
                instructions =
                    Variables.SelectMany(v => v.GenerateInstructions(context))
                    .Prepend(new OpLabel()
                    {
                        Result = context.CreateID()
                    })
                    .Append(new OpBranch()
                    {
                        TargetLabel = context.IDOf(Blocks.First())
                    }).Concat(instructions);
            return instructions;
        }
    }

    internal class EntryFunction : DefinedFunction
    {
        public ExecutionModel ExecutionModel { get; }
        public ISet<Variable> Interface { get; } = new HashSet<Variable>();

        public EntryFunction(string name, ExecutionModel model) : base(name, new SpirvVoidType())
            => ExecutionModel = model;

        public Instruction GenerateEntryPoint(IIDMapper context) => new OpEntryPoint()
        {
            ExecutionModel = ExecutionModel,
            EntryPoint = context.IDOf(this),
            Name = Name,
            Interface = Interface.Select(context.IDOf).ToImmutableArray()
        };
    }
}