using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using System.Collections.Immutable;
using cilspirv.Transpiler.Values;
using cilspirv;

namespace cilspirv.Transpiler.Declarations
{
    internal class Module
    {
        private readonly ObservableHashSet<Capability> capabilities = new ObservableHashSet<Capability>();
        private readonly HashSet<Capability> allCapabilities = new HashSet<Capability>();

        public AddressingModel AddressingModel { get; set; }
        public MemoryModel MemoryModel { get; set; }
        public ISet<Capability> Capabilities => capabilities;
        public ISet<string> Extensions { get; } = new HashSet<string>();
        public ISet<ExtInstructionSet> ExtInstructionSets { get; } = new HashSet<ExtInstructionSet>();
        public ISet<Function> Functions { get; } = new HashSet<Function>();
        public IEnumerable<EntryFunction> EntryPoints => Functions.OfType<EntryFunction>();
        public ISet<Variable> GlobalVariables { get; } = new HashSet<Variable>();

        /// <summary>Available capabilities including implicitly declared ones</summary>
        public IReadOnlySetCapability AllCapabilities
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

        public IEnumerable<Instruction> GenerateInstructions(IIDMapper context)
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

            foreach (var entryPoint in Functions.OfType<EntryFunction>())
                yield return entryPoint.GenerateEntryPoint(context);

            foreach (var entryPoint in Functions.OfType<EntryFunction>().Where(e => e.ExecutionModel == ExecutionModel.Fragment))
                yield return new OpExecutionMode()
                {
                    EntryPoint = context.IDOf(entryPoint),
                    Mode = ExecutionMode.OriginUpperLeft
                };

            // TODO: Allow specification of execution modes

            // In order for the types to be complete and properly orderable we need to
            // first generate all that could request such types
            var functionInstructions = Functions
                .OrderBy(f => f is DefinedFunction ? 1 : -1) // declarations first
                .SelectMany(f => f.GenerateInstructions(context))
                .ToArray();

            var globalVariables = GlobalVariables
                .SelectMany(v => v.GenerateInstructions(context))
                .ToArray();

            var constants = context
                .OfType<Constant>()
                .ToArray() // types requested by constants can change the collection
                .SelectMany(c => c.GenerateInstructions(context))
                .ToArray();

            var types = GetOrderedTypeList(context)
                .SelectMany(t => t.GenerateInstructions(context))
                .ToArray();

            var instructionSets = new List<IEnumerable<Instruction>>();

            if (context.Options.DebugInfo)
            {
                instructionSets.Add(context
                    .OfType<IDebugInstructionGeneratable>()
                    .SelectMany(g => g.GenerateDebugInfo(context))
                    .ToArray());
            }

            instructionSets.Add(context
                .OfType<IDecoratableInstructionGeneratable>()
                .SelectMany(d => d.GenerateDecorations(context))
                .ToArray());

            instructionSets.Add(types);

            instructionSets.Add(constants);

            instructionSets.Add(globalVariables);

            instructionSets.Add(functionInstructions);

            foreach (var instr in instructionSets.SelectMany())
                yield return instr;
        }

        private IEnumerable<SpirvType> GetOrderedTypeList(IIDMapper context)
        {
            var rootTypes = context.OfType<SpirvType>();
            var remainingTypes = rootTypes
                .Concat(rootTypes.SelectMany(t => t.Dependencies))
                .ToHashSet();

            var typeList = new List<SpirvType>();
            while (remainingTypes.Any())
            {
                var ready = remainingTypes
                    .Where(t => t.Dependencies.All(typeList.Contains))
                    .ToArray();
                if (ready.Length == 0)
                    throw new InvalidOperationException("Some type dependency is missing");
                typeList.AddRange(ready);
                foreach (var r in ready)
                    remainingTypes.Remove(r);
            }
            return typeList;
        }
    }
}
