using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Library;
using cilspirv.Spirv;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;

namespace cilspirv.Transpiler
{
    internal interface ITranspilerContext : IInstructionGeneratorContext
    {
        TranspilerLibrary Library { get; }
        TranspilerModule Module { get; }
        TranspilerFunction Function { get; }
    }

    internal interface ITranspilerMethodContext : ITranspilerContext
    {
        (ID id, SpirvType type)? This { get; }
        IReadOnlyList<(ID id, SpirvType type)> Parameters { get; }
        ID ResultID { get; set; }
    }

    internal interface ITranspilerFieldContext : ITranspilerContext
    {
        StackEntry Parent { get; }
        StackEntry Result { set; }
    }

    internal interface ITranspilerFieldBehavior : IMappedFromCILField
    {
        // if LoadAddress is defined (returns not null) Load/Store will use standard OpLoad/OpStore 
        // if LoadAddress is not defined, ldflda is not supported

        IEnumerable<SpirvInstruction>? LoadAddress(ITranspilerFieldContext context) { return null; }
        IEnumerable<SpirvInstruction>? Load(ITranspilerFieldContext context) { return null; }
        IEnumerable<SpirvInstruction>? Store(ITranspilerFieldContext context, ValueStackEntry value) { return null; }
    }

    internal interface ITranspilerLibraryMapper
    {
        GenerateCallDelegate? TryMapMethod(MethodReference methodRef) { return null; }
        IMappedFromCILType? TryMapType(TypeReference ilTypeRef) { return null; }
        ITranspilerFieldBehavior? TryMapFieldBehavior(FieldReference fieldRef) { return null; }
    }

    internal interface IITranspilerLibraryScanner
    {
        StorageClass? TryScanStorageClass(ICustomAttributeProvider fieldDef);
        IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider fieldDef);
    }

    internal delegate IEnumerable<SpirvInstruction> GenerateCallDelegate(ITranspilerMethodContext context);
    internal delegate IEnumerable<SpirvInstruction> GenerateFieldDelegate(ITranspilerFieldContext context);
    internal delegate IEnumerable<SpirvInstruction> GenerateFieldStoreDelegate(ITranspilerFieldContext context, ValueStackEntry value);

    internal interface IMappedFromCILType { }
    internal interface IMappedFromCILField { }
    internal interface IMappedFromCILParam { }

    internal class TranspilerLibrary 
    {
        private readonly TypeDefinition ilModuleType;
        private readonly TranspilerModule module;
        private readonly Action<TranspilerDefinedFunction, MethodBody> queueMethodBody;
        private readonly Dictionary<string, GenerateCallDelegate> mappedMethods = new Dictionary<string, GenerateCallDelegate>();
        private readonly Dictionary<string, IMappedFromCILType> mappedTypes = new Dictionary<string, IMappedFromCILType>();
        private readonly Dictionary<string, IMappedFromCILField> mappedFields = new Dictionary<string, IMappedFromCILField>();
        private readonly Dictionary<string, IMappedFromCILParam> mappedParameters = new Dictionary<string, IMappedFromCILParam>();
        private readonly TranspilerStructMapper structMapper;
        private readonly TranspilerReferenceMapper referenceMapper;
        private readonly TranspilerInternalMethodMapper methodMapper;

        public IEnumerable<ITranspilerLibraryMapper> AllMappers => Mappers.Reverse()
            .Append(structMapper)
            .Append(methodMapper)
            .Prepend(referenceMapper);
        public IEnumerable<IITranspilerLibraryScanner> AllScanners => Scanners.Reverse();

        public IList<ITranspilerLibraryMapper> Mappers { get; } = new List<ITranspilerLibraryMapper>()
        {
            new BuiltinTypeMapper()
        };

        public IList<IITranspilerLibraryScanner> Scanners { get; } = new List<IITranspilerLibraryScanner>()
        {
            new AttributeScanner()
        };

        public TranspilerLibrary(TypeDefinition ilModuleType, TranspilerModule module, Action<TranspilerDefinedFunction, MethodBody> queueMethodBody)
        {
            this.ilModuleType = ilModuleType;
            this.module = module;
            this.queueMethodBody = queueMethodBody;
            structMapper = new TranspilerStructMapper(this, module);
            referenceMapper = new TranspilerReferenceMapper(this);
            methodMapper = new TranspilerInternalMethodMapper(this);
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
            if (mappedFields.TryGetValue(fieldRef.FullName, out var mapped))
                return mapped;

            mapped = MapType(fieldRef.FieldType) switch
            {
                TranspilerVarGroup varGroup => varGroup,
                TranspilerVarGroupTemplate template => InstantiateTemplateFor(fieldRef.FullName, fieldRef.Resolve(), template),
                SpirvType realType when (fieldRef.DeclaringType.FullName == ilModuleType.FullName) => MapGlobalVariable(realType),

                SpirvType realType => MapType(fieldRef.DeclaringType) switch
                {
                    SpirvStructType declaringStructType => declaringStructType.Members.First(m => m.Name == fieldRef.Name),
                    TranspilerVarGroup varGroup => varGroup.Variables.First(v => v.Name == fieldRef.FullName),
                    _ when TryMapFieldBehavior(fieldRef) is ITranspilerFieldBehavior fieldBehavior => fieldBehavior,

                    _ => throw new NotSupportedException("Unsupported field container type")
                },

                _ => throw new NotSupportedException("Unsupported field type")
            };
            mappedFields[fieldRef.FullName] = mapped;
            return mapped;

            TranspilerVariable MapGlobalVariable(SpirvType realType)
            {
                var storageClass = TryScanStorageClass(fieldRef.Resolve())
                    ?? throw new InvalidOperationException($"Could not scan storage class for field {fieldRef.FullName}");
                var decorations = ScanDecorations(fieldRef.Resolve());

                var variable = new TranspilerVariable(fieldRef.FullName, new SpirvPointerType()
                {
                    Type = realType,
                    StorageClass = storageClass
                })
                {
                    Decorations = decorations.ToHashSet()
                };

                module.GlobalVariables.Add(variable);
                return variable;
            }
        }

        private TranspilerVarGroup InstantiateTemplateFor(string elementName, ICustomAttributeProvider element, TranspilerVarGroupTemplate template)
        {
            var storageClass = TryScanStorageClass(element);
            if (storageClass == null)
                throw new InvalidOperationException("Field has to have a storage class to instantiate a var group template");

            return structMapper.MapVarGroup($"{elementName}#VarGroup", template.TypeDefinition, storageClass);
        }

        private ITranspilerFieldBehavior? TryMapFieldBehavior(FieldReference fieldRef) => AllMappers
            .Select(mapper => mapper.TryMapFieldBehavior(fieldRef))
            .FirstOrDefault();

        private IEnumerable<DecorationEntry> ScanDecorations(ICustomAttributeProvider element) => AllScanners
            .Select(scanner => scanner.TryScanDecorations(element))
            .FirstOrDefault(c => c.Any())
            ?? Enumerable.Empty<DecorationEntry>();

        private StorageClass? TryScanStorageClass(ICustomAttributeProvider element) => AllScanners
            .Select(scanner => scanner.TryScanStorageClass(element))
            .FirstOrDefault(c => c.HasValue);

        public IMappedFromCILParam MapParameter(ParameterDefinition paramDef, TranspilerFunction function)
        {
            var mappingName = $"{function.Name}#{paramDef.Name}";
            if (mappedParameters.TryGetValue(mappingName, out var mapped))
                return mapped;

            var paramType = MapType(paramDef.ParameterType);
            var storageClass = TryScanStorageClass(paramDef);
            var decorations = ScanDecorations(paramDef).ToHashSet();

            mapped = paramType switch
            {
                TranspilerVarGroup varGroup => varGroup,
                TranspilerVarGroupTemplate template => InstantiateTemplateFor(mappingName, paramDef, template),
                SpirvType realType when storageClass != null => MapGlobalVariable(realType),
                SpirvType realType => MapSpirvParameter(realType),
                _ => throw new NotSupportedException("Unsupported parameter type")
            };
            mappedParameters[mappingName] = mapped;
            return mapped;

            IMappedFromCILParam MapGlobalVariable(SpirvType realType)
            {
                var variable = new TranspilerVariable(paramDef.Name, new SpirvPointerType()
                {
                    Type = realType,
                    StorageClass = storageClass.Value,
                })
                {
                    Decorations = decorations
                };
                module.GlobalVariables.Add(variable);
                return variable;
            }

            TranspilerParameter MapSpirvParameter(SpirvType realType)
            {
                if (function is TranspilerEntryFunction)
                    throw new InvalidOperationException("Entry point parameters require a storage class");
                var parameter = new TranspilerParameter(function.Parameters.Count, paramDef.Name, realType)
                {
                    Decorations = decorations
                };
                function.Parameters.Add(parameter);
                return parameter;
            }
        }

        public TranspilerFunction? TryMapInternalMethod(MethodDefinition ilMethod, bool isEntryPoint)
        {
            var returnType = MapType(ilMethod.ReturnType);
            if (isEntryPoint)
            {
                if (!ilMethod.HasBody)
                    throw new InvalidOperationException("An entry point method has to have a body");
                if (returnType is not SpirvVoidType && returnType is not TranspilerVarGroup)
                    throw new InvalidOperationException("An entry point can only return void or a variable group");
            }
            else
            {
                if (returnType is not SpirvType)
                    throw new InvalidOperationException("A function can only return SPIRV types");
            }

            var function =
                isEntryPoint ? new TranspilerEntryFunction(ilMethod.Name, ExtractExecutionModel(ilMethod))
                : ilMethod.HasBody ? new TranspilerDefinedFunction(ilMethod.Name, (SpirvType)returnType)
                : new TranspilerFunction(ilMethod.Name, (SpirvType)returnType);

            if (ilMethod.HasThis && ilMethod.DeclaringType.FullName != ilModuleType.FullName)
                throw new NotSupportedException("Real instance methods are not supported yet");

            foreach (var parameter in ilMethod.Parameters)
                MapParameter(parameter, function);

            module.Functions.Add(function);
            if (function is TranspilerDefinedFunction definedFunction)
                queueMethodBody(definedFunction, ilMethod.Body);
            return function;
        }

        private ExecutionModel ExtractExecutionModel(MethodDefinition ilMethod)
        {
            var attr = ilMethod.GetCustomAttributes<EntryPointAttribute>().SingleOrDefault();
            if (attr == null)
                throw new InvalidOperationException($"Entry point method {ilMethod.FullName} does not have an EntryPointAttribute");

            return (ExecutionModel)attr.ConstructorArguments.Single().Value;
        }
    }
}
