using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using System.Collections.Immutable;

namespace cilspirv.Transpiler
{

    internal class TranspilerModule : IInstructionGeneratable
    {
        private readonly ObservableHashSet<Capability> capabilities = new ObservableHashSet<Capability>();
        private readonly HashSet<Capability> allCapabilities = new HashSet<Capability>();

        public AddressingModel AddressingModel { get; set; }
        public MemoryModel MemoryModel { get; set; }
        public ISet<Capability> Capabilities => capabilities;
        public ISet<string> Extensions { get; } = new HashSet<string>();
        public ISet<TranspilerExtInstructionSet> ExtInstructionSets { get; } = new HashSet<TranspilerExtInstructionSet>();
        public ISet<TranspilerType> Types { get; } = new HashSet<TranspilerType>();
        public ISet<TranspilerFunction> Functions { get; } = new HashSet<TranspilerFunction>();
        public IEnumerable<TranspilerEntryFunction> EntryPoints => Functions.OfType<TranspilerEntryFunction>();
        public ISet<TranspilerVariable> GlobalVariables { get; } = new HashSet<TranspilerVariable>();

        /// <summary>Available capabilities including implicitly declared ones</summary>
        public IReadOnlySet<Capability> AllCapabilities
        {
            get
            {
                if (!capabilities.ResetHasChanged())
                    return allCapabilities;

                allCapabilities.Clear();
                var queue = new Queue<Capability>(capabilities);
                var capEnum = typeof(Capability);
                while (queue.TryDequeue(out var cap))
                {
                    if (allCapabilities.Contains(cap))
                        continue;
                    var capMember = capEnum.GetMember(cap.ToString()).Single();
                    var dependCaps = capMember
                        .GetCustomAttributes(typeof(DependsOnAttribute), inherit: false)
                        .SelectMany(attr => ((DependsOnAttribute)attr).Capabilities)
                        .Where(dependCap => !allCapabilities.Contains(dependCap))
                        .Distinct();
                    foreach (var dependCap in dependCaps)
                        queue.Enqueue(dependCap);
                }
                return allCapabilities;
            }
        }

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            foreach (var cap in Capabilities)
                yield return new OpCapability()
                {
                    Capability = cap
                };

            foreach (var ext in Extensions)
                yield return new OpExtension()
                {
                    Name = ext
                };

            foreach (var extInst in ExtInstructionSets.SelectMany(set => set.GenerateInstructions(context)))
                yield return extInst;

            yield return new OpMemoryModel()
            {
                AddressingModel = AddressingModel,
                MemoryModel = MemoryModel
            };

            foreach (var entryPoint in Functions.OfType<TranspilerEntryFunction>())
                yield return entryPoint.GenerateEntryPoint(context);

            // TODO: Execution modes

            // For the rest we need lowest level, the function blocks generated, so that OfType has all references
            var functionDefs = Functions.OfType<TranspilerDefinedFunction>();
            var functionInstructions =
                Functions.Except(functionDefs).SelectMany(f => f.GenerateInstructions(context))
                .Concat(functionDefs.SelectMany(f => f.GenerateInstructions(context)))
                .ToImmutableArray();

            var instructionSets = new List<IEnumerable<Instruction>>();

            if (context.ShouldGenerateDebugInfo)
            {
                instructionSets.Add(context
                    .OfType<IDebugInstructionGeneratable>()
                    .SelectMany(g => g.GenerateDebugInfo(context)));
            }

            instructionSets.Add(context
                .OfType<IDecoratableInstructionGeneratable>()
                .SelectMany(d => d.GenerateDecorations(context)));

            instructionSets.Add(context
                .OfType<TranspilerType>()
                .SelectMany(t => t.GenerateInstructions(context)));

            instructionSets.Add(GlobalVariables
                .SelectMany(v => v.GenerateInstructions(context)));

            instructionSets.Add(functionInstructions);

            foreach (var instr in instructionSets.SelectMany())
                yield return instr;
        }
    }
}
