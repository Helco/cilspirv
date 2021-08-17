using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Library;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal interface ITranspilerMethodContext : IInstructionGeneratorContext
    {
        TranspilerLibrary Library { get; }
        TranspilerModule Module { get; }
    }

    internal interface ITranspilerLibraryMapper
    {
        GenerateCallDelegate? TryMapMethod(MethodReference methodRef);
        IMappedFromCILType? TryMapType(TypeReference ilTypeRef);
    }

    internal interface IITranspilerLibraryScanner
    {
        StorageClass? TryScanStorageClass(ICustomAttributeProvider fieldDef);
        IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider fieldDef);
    }

    internal delegate IEnumerable<Instruction> GenerateCallDelegate(ITranspilerMethodContext context, IReadOnlyList<(ID id, SpirvType type)> parameters, out ID? resultId);

    internal interface IMappedFromCILType { }
    internal interface IMappedFromCILField { }

    internal class TranspilerLibrary 
    {
        private readonly TypeDefinition ilModuleType;
        private readonly TranspilerModule module;
        private readonly Dictionary<string, GenerateCallDelegate> mappedMethods = new Dictionary<string, GenerateCallDelegate>();
        private readonly Dictionary<string, IMappedFromCILType> mappedTypes = new Dictionary<string, IMappedFromCILType>();
        private readonly Dictionary<string, IMappedFromCILField> mappedFields = new Dictionary<string, IMappedFromCILField>();
        private readonly TranspilerStructMapper structMapper;

        public IEnumerable<ITranspilerLibraryMapper> AllMappers => Mappers.Reverse().Append(structMapper);
        public IEnumerable<IITranspilerLibraryScanner> AllScanners => Scanners.Reverse();

        public IList<ITranspilerLibraryMapper> Mappers { get; } = new List<ITranspilerLibraryMapper>()
        {
            new BuiltinTypeMapper()
        };

        public IList<IITranspilerLibraryScanner> Scanners { get; } = new List<IITranspilerLibraryScanner>()
        {
            new AttributeScanner()
        };

        public TranspilerLibrary(TypeDefinition ilModuleType, TranspilerModule module)
        {
            this.ilModuleType = ilModuleType;
            this.module = module;
            structMapper = new TranspilerStructMapper(this, module);
        }

        public IMappedFromCILType? TryMapType(TypeReference ilTypeRef)
        {
            if (mappedTypes.TryGetValue(ilTypeRef.FullName, out var mappedType))
                return mappedType;

            mappedType = AllMappers
                .Select(mapper => mapper.TryMapType(ilTypeRef))
                .FirstOrDefault(m => m != null);
            if (mappedType == null)
                return mappedType;

            mappedTypes.Add(ilTypeRef.FullName, mappedType);
            return mappedType;
        }

        public GenerateCallDelegate? TryMapMethod(MethodReference ilMethodRef)
        {
            if (mappedMethods.TryGetValue(ilMethodRef.FullName, out var mapped))
                return mapped;

            mapped = AllMappers
                .Select(mapper => mapper.TryMapMethod(ilMethodRef))
                .FirstOrDefault(m => m != null);
            if (mapped == null)
                return mapped;

            mappedMethods.Add(ilMethodRef.FullName, mapped);
            return mapped;
        }

        public GenerateCallDelegate MapMethod(MethodReference ilMethodRef) => TryMapMethod(ilMethodRef)
            ?? throw new ArgumentException($"Cannot map method {ilMethodRef.FullName}");

        public IMappedFromCILType MapType(TypeReference ilTypeRef) => TryMapType(ilTypeRef)
            ?? throw new ArgumentException($"Cannot map type {ilTypeRef.FullName}");

        public IMappedFromCILField MapField(FieldReference fieldRef)
        {
            if (mappedFields.TryGetValue(fieldRef.FullName, out var prevMapped))
                return prevMapped;

            switch(MapType(fieldRef.FieldType))
            {
                case TranspilerVarGroup varGroup:
                    var variable = varGroup.Variables.First(v => v.Name == fieldRef.FullName);
                    mappedFields.Add(fieldRef.FullName, variable);
                    return variable;

                case SpirvType realType when (fieldRef.DeclaringType.FullName == ilModuleType.FullName):
                    return MapGlobalVariable(realType);

                case SpirvType realType:
                    var declaringType = MapType(fieldRef.DeclaringType);
                    if (declaringType is not SpirvStructType declaringStructType)
                        throw new InvalidOperationException("Unexpected parent type for real field");

                    return declaringStructType.Members.First(m => m.Name == fieldRef.Name);

                default: throw new NotSupportedException("Unsupported but mapped field type");
            }

            TranspilerVariable MapGlobalVariable(SpirvType realType)
            {
                var storageClass = AllScanners
                    .Select(scanner => scanner.TryScanStorageClass(fieldRef.Resolve()))
                    .FirstOrDefault(c => c.HasValue)
                    ?? throw new InvalidOperationException($"Could not scan storage class for field {fieldRef.FullName}");
                var decorations = AllScanners
                    .Select(scanner => scanner.TryScanDecorations(fieldRef.Resolve()))
                    .FirstOrDefault(c => c.Any())
                    ?? Enumerable.Empty<DecorationEntry>();

                var variable = new TranspilerVariable(fieldRef.FullName, new SpirvPointerType()
                {
                    Type = realType,
                    StorageClass = storageClass,
                    Decorations = decorations.ToHashSet()
                });

                module.GlobalVariables.Add(variable);
                return variable;
            }
        }
    }
}
