using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerFunction : IDecoratableInstructionGeneratable, IDebugInstructionGeneratable
    {
        public string Name { get; }
        public SpirvType ReturnType { get; set; }
        public IList<TranspilerParameter> Parameters { get; } = new List<TranspilerParameter>();
        public IReadOnlySet<DecorationEntry> Decorations { get; set; } = new HashSet<DecorationEntry>();
        public FunctionControl FunctionControl { get; set; }

        public SpirvFunctionType SpirvFunctionType => new SpirvFunctionType()
        {
            ReturnType = ReturnType,
            ParameterTypes = Parameters.Select(p => p.Type).ToImmutableArray()
        };

        public TranspilerFunction(string name, SpirvType returnType) =>
            (Name, ReturnType) = (name, returnType);

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context) =>
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

        protected virtual IEnumerable<Instruction> GenerateBody(IInstructionGeneratorContext context) => Enumerable.Empty<Instruction>();

        public IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context)
        {
            yield return new OpName()
            {
                Target = context.IDOf(this),
                Name = Name
            };
        }
    }

    internal class TranspilerDefinedFunction : TranspilerFunction
    {
        public IList<TranspilerBlock> Blocks { get; } = new List<TranspilerBlock>();
        public IList<TranspilerVariable> Variables { get; } = new List<TranspilerVariable>();

        public TranspilerDefinedFunction(string name, SpirvType returnType) : base(name, returnType) { }

        protected override IEnumerable<Instruction> GenerateBody(IInstructionGeneratorContext context)
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

    internal class TranspilerEntryFunction : TranspilerDefinedFunction
    {
        public ExecutionModel ExecutionModel { get; }
        public ISet<TranspilerVariable> Interface { get; } = new HashSet<TranspilerVariable>();

        public TranspilerEntryFunction(string name, ExecutionModel model) : base(name, new SpirvVoidType())
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