using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;
using cilspirv.Transpiler.Values;
using cilspirv;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler.BuiltInLibrary
{
    internal class StructMapper : ITranspilerLibraryMapper
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
        private readonly Module module;

        public StructMapper(TranspilerLibrary library, Module module)
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
            var instanceFields = ilTypeDef.Fields.Where(f => !f.IsStatic);
            var fieldStorageClasses = instanceFields
                .Select(field => (field, ScanStorageClass(field)))
                .ToDictionary(t => t.field, t => t.Item2);
            var fieldDecorations = instanceFields
                .SelectMany(field => ScanDecorations(field).Select(decoration => (field, decoration)))
                .ToLookup(t => t.field, t => t.decoration);

            var fieldsHaveStorageClass = fieldStorageClasses.Values.Any(v => v.HasValue);
            var fieldsHaveGlobalDecorations = fieldDecorations.Any(g => g.Any(d => GlobalDecorations.Contains(d.Kind)));
            var fieldsHaveMemberDecorations = fieldDecorations.Any(g => g.Any(d => MemberDecorations.Contains(d.Kind)));
            if (fieldsHaveMemberDecorations && (fieldsHaveStorageClass || fieldsHaveGlobalDecorations))
                throw new InvalidOperationException($"Type \"{ilTypeDef.FullName}\" is ambiguous whether it contains global variables or members");

            IMappedFromCILType structure =
                fieldsHaveStorageClass ? MapVarGroup(ilTypeDef.Name, ilTypeDef, structStorageClass)
                : fieldsHaveGlobalDecorations ? new VarGroupTemplate(ilTypeDef)
                : MapStructure();

            structures.Add(ilTypeDef.FullName, structure);
            return structure;

            SpirvStructType MapStructure() => new SpirvStructType()
            {
                UserName = ilTypeDef.Name,
                Decorations = structureDecorations.ToHashSet(),
                Members = instanceFields.Select(MapStructureMember).ToImmutableArray()
            };

            StructMember MapStructureMember(FieldDefinition fieldDef, int index) => new StructMember(
                index,
                fieldDef.Name,
                library.MapType(fieldDef.FieldType) as SpirvType ??
                    throw new InvalidOperationException("A structure can only hold fields to other SPIRV types"),
                fieldDecorations[fieldDef].ToImmutableHashSet());
        }

        internal VarGroup MapVarGroup(string name, TypeDefinition ilTypeDef, StorageClass? structStorageClass)
        {
            var instanceFields = ilTypeDef.Fields.Where(f => !f.IsStatic);
            var fieldStorageClasses = instanceFields
                .Select(field => (field, ScanStorageClass(field)))
                .ToDictionary(t => t.field, t => t.Item2);
            var fieldDecorations = instanceFields
                .SelectMany(field => ScanDecorations(field).Select(decoration => (field, decoration)))
                .ToLookup(t => t.field, t => t.decoration);

            var missingStorageClassFields = fieldStorageClasses.Where(kv => !kv.Value.HasValue);
            if (structStorageClass == null && missingStorageClassFields.Any())
                throw new InvalidOperationException($"Global variable type \"{ilTypeDef.FullName}\" missing storage classes for " +
                    string.Join(", ", missingStorageClassFields.Select(kv => kv.Key.Name)));

            var varGroup = new VarGroup(name, ilTypeDef);
            foreach (var field in instanceFields)
            {
                var fieldType = library.MapType(field.FieldType);
                if (fieldType is VarGroup subVarGroup)
                {
                    varGroup.Variables.AddRange(subVarGroup.Variables);
                    continue;
                }
                if (fieldType is not SpirvType fieldSpirvType)
                    throw new InvalidOperationException("Invalid field type in variable group structure");

                var variable = new GlobalVariable(field.FullName, new SpirvPointerType()
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

        private IEnumerable<DecorationEntry> ScanDecorations(ICustomAttributeProvider fieldDef) => library.AllMappers
            .Select(scanner => scanner.TryScanDecorations(fieldDef))
            .FirstOrDefault(Enumerable.Any)
            ?? Enumerable.Empty<DecorationEntry>();

        private StorageClass? ScanStorageClass(ICustomAttributeProvider fieldDef) => library.AllMappers
            .Select(scanner => scanner.TryScanStorageClass(fieldDef))
            .FirstOrDefault(s => s.HasValue);
    }
}