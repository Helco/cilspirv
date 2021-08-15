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
        TranspilerType? TryMapType(TypeReference ilTypeRef);
    }

    internal interface IITranspilerLibraryScanner
    {
        StorageClass? TryScanStorageClass(FieldReference fieldRef);
        StorageClass? TryScanStorageClass(ParameterReference parameterRef);
        StorageClass? TryScanStorageClass(MethodReturnType returnType);
        IEnumerable<DecorationEntry> TryScanDecorations(FieldReference fieldRef);
        IEnumerable<DecorationEntry> TryScanDecorations(ParameterReference parameterRef);
        IEnumerable<DecorationEntry> TryScanDecorations(MethodReturnType returnType);
    }

    internal delegate IEnumerable<Instruction> GenerateCallDelegate(ITranspilerMethodContext context, IReadOnlyList<(ID id, SpirvType type)> parameters, out ID? resultId);

    internal class TranspilerLibrary 
    {
        private readonly Lazy<AssemblyDefinition> myAssembly;
        private readonly TranspilerModule module;
        private readonly Dictionary<string, GenerateCallDelegate> mappedMethods = new Dictionary<string, GenerateCallDelegate>();
        private readonly Dictionary<string, TranspilerType> mappedTypes = new Dictionary<string, TranspilerType>();

        // TODO: used later to add module-defined mappings
        private IEnumerable<ITranspilerLibraryMapper> AllMappers => Mappers;

        public IList<ITranspilerLibraryMapper> Mappers { get; } = new List<ITranspilerLibraryMapper>()
        {
            new BuiltinTypeMapper()
        };

        public IList<IITranspilerLibraryScanner> Scanners { get; } = new List<IITranspilerLibraryScanner>()
        {
            new AttributeScanner()
        };

        public TranspilerLibrary(TranspilerModule module)
        {
            this.module = module;
            myAssembly = new Lazy<AssemblyDefinition>(
                () => AssemblyDefinition.ReadAssembly(typeof(Transpiler).Assembly.Location));
        }

        public TranspilerType? TryMapType(TypeReference ilTypeRef)
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

        public TranspilerType MapType(TypeReference ilTypeRef) => TryMapType(ilTypeRef)
            ?? throw new ArgumentException($"Cannot map type {ilTypeRef.FullName}");

        public TranspilerType MapType<T>() =>
            MapType(myAssembly.Value.MainModule.ImportReference(typeof(T)));

        public TranspilerVariable MapVariable(FieldReference fieldRef)
        {
            var variable = module.GlobalVariables.FirstOrDefault(v => v.Name == fieldRef.Name);
            if (variable != null)
                return variable;

            var fieldType = MapType(fieldRef.FieldType);
            var storageClass = Scanners
                .Select(scanner => scanner.TryScanStorageClass(fieldRef))
                .FirstOrDefault(c => c.HasValue)
                ?? throw new InvalidOperationException($"Could not scan storage class for field {fieldRef.FullName}");
            var decorations = Scanners
                .Select(scanner => scanner.TryScanDecorations(fieldRef))
                .FirstOrDefault(c => c.Any())
                ?? Enumerable.Empty<DecorationEntry>();

            variable = new TranspilerVariable(fieldRef.Name, new SpirvPointerType()
            {
                Type = fieldType.Type,
                StorageClass = storageClass,
            });
            foreach (var decoration in decorations)
                variable.Decorations.Add(decoration);

            module.GlobalVariables.Add(variable);
            return variable;
        }
    }
}
