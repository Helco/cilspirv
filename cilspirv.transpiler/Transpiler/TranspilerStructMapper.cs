using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Library;
using cilspirv.Spirv;
using System.Runtime.InteropServices;

namespace cilspirv.Transpiler
{
    internal class TranspilerStructMapper : ITranspilerLibraryMapper
    {
        private static readonly ISet<Decoration> CollapsingDecorations = new HashSet<Decoration>()
        {
            Decoration.SpecId,
            Decoration.Block,
            Decoration.Location,
            Decoration.Binding,
            Decoration.DescriptorSet
        };
        private static readonly ISet<Decoration> MemberDecorations = new HashSet<Decoration>()
        {
            Decoration.Offset
        };

        private readonly Dictionary<string, TranspilerStructType> structures = new Dictionary<string, TranspilerStructType>();
        private readonly TranspilerLibrary library;
        private readonly TranspilerModule module;

        public TranspilerStructMapper(TranspilerLibrary library, TranspilerModule module)
        {
            this.library = library;
            this.module = module;
        }

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;

        public TranspilerType? TryMapType(TypeReference ilTypeRef)
        {
            if (structures.TryGetValue(ilTypeRef.FullName, out var prevMapped))
                return prevMapped;
            var ilTypeDef = ilTypeRef.Resolve();
            if (!ilTypeDef.IsValueType || ilTypeDef.IsPrimitive || ilTypeDef.IsEnum || !ilTypeDef.HasFields)
                return null;

            var fieldStorageClasses = ilTypeDef.Fields
                .Select(field => (field, ScanStorageClass(field)))
                .ToDictionary(t => t.field, t => t.Item2);
            var fieldDecorations = ilTypeDef.Fields
                .SelectMany(field => ScanDecorations(field).Select(decoration => (field, decoration)))
                .ToLookup(t => t.field, t => t.decoration);
            var structure = IsCollapsingStruct()
                ? MapCollapsingStruct()
                : MapRealStruct();

            AddAllDecorations();

            structures.Add(ilTypeDef.FullName, structure);
            return structure;

            bool IsCollapsingStruct()
            {
                var countStorageFields = fieldStorageClasses.Count(kv => kv.Value.HasValue);
                var countCollapsingFields = fieldDecorations.Count(
                    group => group.Any(e => CollapsingDecorations.Contains(e.Kind)));
                var countMemberFields = fieldDecorations.Count(
                    group => group.Any(e => MemberDecorations.Contains(e.Kind)));

                var shouldBeCollapsing = countStorageFields > 0 || countCollapsingFields > 0;
                var shouldBeReal = countMemberFields > 0;
                if (shouldBeCollapsing && shouldBeReal)
                    throw new InvalidOperationException($"Type \"{ilTypeDef.FullName}\" is ambiguous whether it contains global variables or members");
                return shouldBeCollapsing; // so no decorations at all is still a real struct
            }

            TranspilerStructType MapCollapsingStruct()
            {
                var structStorageClass = ScanStorageClass(ilTypeDef);
                var missingStorageClassFields = fieldStorageClasses.Where(kv => !kv.Value.HasValue);
                if (structStorageClass == null && missingStorageClassFields.Any())
                    throw new InvalidOperationException($"Global variable type \"{ilTypeDef.FullName}\" missing storage classes for " +
                        string.Join(", ", missingStorageClassFields.Select(kv => kv.Key.Name)));

                var structure = new TranspilerStructType(isReal: false, ilTypeDef.FullName)
                {
                    DefaultStorageClass = structStorageClass
                };
                foreach (var field in ilTypeDef.Fields)
                {
                    var fieldType = library.MapType(field.FieldType);
                    var variable = new TranspilerVariable(field.FullName, new SpirvPointerType()
                    {
                        Type = fieldType.Type,
                        StorageClass = (fieldStorageClasses[field] ?? structStorageClass)!.Value
                    });
                    var member = new TranspilerMember(field.Name, fieldType.Type)
                    {
                        GlobalVariable = variable
                    };
                    structure.Members.Add(member);
                    module.GlobalVariables.Add(variable);
                }

                return structure;
            }

            TranspilerStructType MapRealStruct()
            {
                var structure = new TranspilerStructType(isReal: true, ilTypeDef.FullName);
                foreach (var field in ilTypeDef.Fields)
                    structure.Members.Add(new TranspilerMember(
                        field.Name,
                        library.MapType(field.FieldType).Type));
                return structure;
            }

            void AddAllDecorations()
            {
                var structureDecorations = ScanDecorations(ilTypeDef);
                foreach (var decoration in structureDecorations)
                    structure.Decorations.Add(decoration);
                for (int i = 0; i < ilTypeDef.Fields.Count; i++)
                {
                    var member = structure.Members[i];
                    var field = ilTypeDef.Fields[i];
                    foreach (var decoration in fieldDecorations[field])
                    {
                        member.Decorations.Add(decoration);
                        member.GlobalVariable?.Decorations.Add(decoration);
                    }
                }
            }
        }

        private IEnumerable<DecorationEntry> ScanDecorations(ICustomAttributeProvider fieldDef) => library.AllScanners
            .Select(scanner => scanner.TryScanDecorations(fieldDef))
            .FirstOrDefault(Enumerable.Any)
            ?? Enumerable.Empty<DecorationEntry>();

        private StorageClass? ScanStorageClass(ICustomAttributeProvider fieldDef) => library.AllScanners
            .Select(scanner => scanner.TryScanStorageClass(fieldDef))
            .FirstOrDefault(s => s.HasValue);
    }
}