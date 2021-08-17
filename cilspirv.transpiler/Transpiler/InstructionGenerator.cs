using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class InstructionGenerator : IInstructionGeneratorContext
    {
        private const uint TemporaryIDThreshold = 0xA0000000;

        private readonly Dictionary<IInstructionGeneratable, ID> permIDs = new Dictionary<IInstructionGeneratable, ID>();
        private readonly Dictionary<IInstructionGeneratable, ID> tempIDs = new Dictionary<IInstructionGeneratable, ID>();
        private readonly IDictionary<ID, ID> idRemap = new Dictionary<ID, ID>();
        private uint nextID = 1;
        private uint nextTemporaryID = uint.MaxValue;

        public uint Bound => nextID;

        public TranspilerOptions Options { get; set; } = new TranspilerOptions();

        public ID CreateID()
        {
            if (nextID >= TemporaryIDThreshold)
                throw new InvalidOperationException("Too many permanent IDs");
            return new ID(nextID++);
        }

        public ID CreateIDFor(IInstructionGeneratable generatable)
        {
            if (permIDs.ContainsKey(generatable))
                throw new InvalidOperationException("Generatable already has an permanent ID");
            if (tempIDs.TryGetValue(generatable, out var tempID))
            {
                var permID = CreateID();
                permIDs[generatable] = permID;
                idRemap[tempID] = permID;
                return permID;
            }
            return permIDs[generatable] = CreateID();
        }

        public ID IDOf(IInstructionGeneratable generatable)
        {
            if (permIDs.TryGetValue(generatable, out var permID))
                return permID;
            if (tempIDs.TryGetValue(generatable, out var tempID))
                return tempID;
            if (nextTemporaryID < TemporaryIDThreshold)
                throw new InvalidOperationException("Too many temporary IDs");
            tempID = new ID(nextTemporaryID--);
            tempIDs[generatable] = tempID;
            return tempID;
        }

        public IEnumerable<T> OfType<T>() where T : IInstructionGeneratable =>
            permIDs.Keys.OfType<T>().ToArray()
            .Concat(tempIDs
                .Where(p => !idRemap.ContainsKey(p.Value) && p.Key is T)
                .Select(p => (T)p.Key));

        public uint MapFromTemporaryID(ID id) => IsTemporaryID(id)
            ? idRemap.TryGetValue(id, out var permId) ? permId.Value : throw new InvalidOperationException("ID was never remapped")
            : id.Value;

        private static bool IsTemporaryID(ID id) => id.Value >= TemporaryIDThreshold;
    }
}
