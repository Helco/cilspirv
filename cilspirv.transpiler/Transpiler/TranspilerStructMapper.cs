using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;


namespace cilspirv.Transpiler
{
    internal class TranspilerStructMapper : ITranspilerLibraryMapper
    {
        private static readonly ISet<Decoration> GlobalDecorations = new HashSet<Decoration>()
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

        private readonly Dictionary<string, IMappedFromCILType> structures = new Dictionary<string, IMappedFromCILType>();
        private readonly TranspilerLibrary library;
        private readonly TranspilerModule module;

        public TranspilerStructMapper(TranspilerLibrary library, TranspilerModule module)
        {
            this.library = library;
            this.module = module;
        }

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;

        public IMappedFromCILType? TryMapType(TypeReference ilTypeRef)
        {
            if (structures.TryGetValue(ilTypeRef.FullName, out var prevMapped))
                return prevMapped;
            var ilTypeDef = ilTypeRef.Resolve();
            if (!ilTypeDef.IsValueType || ilTypeDef.IsPrimitive || ilTypeDef.IsEnum || !ilTypeDef.HasFields)
                return null;

            var structStorageClass = ScanStorageClass(ilTypeDef);
            var structureDecorations = ScanDecorations(ilTypeDef);
            var fieldStorageClasses = ilTypeDef.Fields
                .Select(field => (field, ScanStorageClass(field)))
                .ToDictionary(t => t.field, t => t.Item2);
            var fieldDecorations = ilTypeDef.Fields
                .SelectMany(field => ScanDecorations(field).Select(decoration => (field, decoration)))
                .ToLookup(t => t.field, t => t.decoration);

            var fieldsHaveStorageClass = fieldStorageClasses.Values.Any(v => v.HasValue);
            var fieldsHaveGlobalDecorations = fieldDecorations.Any(g => g.Any(d => GlobalDecorations.Contains(d.Kind)));
            var fieldsHaveMemberDecorations = fieldDecorations.Any(g => g.Any(d => MemberDecorations.Contains(d.Kind)));
            if (fieldsHaveMemberDecorations && (fieldsHaveStorageClass || fieldsHaveGlobalDecorations))
                throw new InvalidOperationException($"Type \"{ilTypeDef.FullName}\" is ambiguous whether it contains global variables or members");

            IMappedFromCILType structure =
                fieldsHaveStorageClass ? MapVarGroup(ilTypeDef.Name, ilTypeDef, structStorageClass)
                : fieldsHaveGlobalDecorations ? new TranspilerVarGroupTemplate(ilTypeDef)
                : MapStructure();

            structures.Add(ilTypeDef.FullName, structure);
            return structure;

            SpirvStructType MapStructure() => new SpirvStructType()
            {
                UserName = ilTypeDef.Name,
                Decorations = structureDecorations.ToHashSet(),
                Members = ilTypeDef.Fields.Select(MapStructureMember).ToImmutableArray()
            };

            SpirvMember MapStructureMember(FieldDefinition fieldDef, int index) => new SpirvMember(
                index,
                fieldDef.Name,
                library.MapType(fieldDef.FieldType) as SpirvType ??
                    throw new InvalidOperationException("A structure can only hold fields to other SPIRV types"),
                fieldDecorations[fieldDef].ToImmutableHashSet());
        }

        internal TranspilerVarGroup MapVarGroup(string name, TypeDefinition ilTypeDef, StorageClass? structStorageClass)
        {
            var fieldStorageClasses = ilTypeDef.Fields
                .Select(field => (field, ScanStorageClass(field)))
                .ToDictionary(t => t.field, t => t.Item2);
            var fieldDecorations = ilTypeDef.Fields
                .SelectMany(field => ScanDecorations(field).Select(decoration => (field, decoration)))
                .ToLookup(t => t.field, t => t.decoration);

            var missingStorageClassFields = fieldStorageClasses.Where(kv => !kv.Value.HasValue);
            if (structStorageClass == null && missingStorageClassFields.Any())
                throw new InvalidOperationException($"Global variable type \"{ilTypeDef.FullName}\" missing storage classes for " +
                    string.Join(", ", missingStorageClassFields.Select(kv => kv.Key.Name)));

            var varGroup = new TranspilerVarGroup(name, ilTypeDef);
            foreach (var field in ilTypeDef.Fields)
            {
                var fieldType = library.MapType(field.FieldType);
                if (fieldType is TranspilerVarGroup subVarGroup)
                {
                    varGroup.Variables.AddRange(subVarGroup.Variables);
                    continue;
                }
                if (fieldType is not SpirvType fieldSpirvType)
                    throw new InvalidOperationException("Invalid field type in variable group structure");

                var variable = new TranspilerVariable(field.FullName, new SpirvPointerType()
                {
                    Type = fieldSpirvType,
                    StorageClass = (fieldStorageClasses[field] ?? structStorageClass)!.Value,
                })
                {
                    Decorations = fieldDecorations[field]
                        .Concat(ScanDecorations(ilTypeDef))
                        .GroupBy(d => d.Kind)
                        .Select(g => g.First())
                        .ToImmutableHashSet()
                };
                varGroup.Variables.Add(variable);
                module.GlobalVariables.Add(variable);
            }

            return varGroup;
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