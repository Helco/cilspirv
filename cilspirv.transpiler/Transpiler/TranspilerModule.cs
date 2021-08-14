using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class TranspilerModule
    {
        private readonly ObservableHashSet<Capability> capabilities = new ObservableHashSet<Capability>();
        private readonly HashSet<Capability> allCapabilities = new HashSet<Capability>();

        public AddressingModel AddressingModel { get; set; }
        public MemoryModel MemoryModel { get; set; }
        public ISet<Capability> Capabilities => capabilities;
        public ISet<string> Extensions { get; } = new HashSet<string>();
        public ISet<string> ExtInstructionSets { get; } = new HashSet<string>();
        public ISet<SpirvType> Types { get; } = new HashSet<SpirvType>();
        public ISet<TranspilerFunction> Functions { get; } = new HashSet<TranspilerFunction>();
        public IEnumerable<TranspilerEntryFunction> EntryPoints => Functions.OfType<TranspilerEntryFunction>();

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
    }
}
