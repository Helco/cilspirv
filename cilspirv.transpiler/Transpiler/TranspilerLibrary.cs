using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Library;
using cilspirv.Spirv;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;
using System.Collections.Immutable;
using cilspirv.Transpiler.Values;
using cilspirv.Transpiler.BuiltInLibrary;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler
{
    internal interface ITranspilerContext : IIDMapper
    {
        TranspilerLibrary Library { get; }
        Module Module { get; }
        Function Function { get; }
    }

    internal interface ITranspilerMethodContext : ITranspilerContext
    {
        (ID id, SpirvType type)? This { get; }
        IReadOnlyList<(ID id, SpirvType type)> Parameters { get; }
        ID ResultID { get; set; }
    }

    internal interface ITranspilerValueContext : ITranspilerContext
    {

        StackEntry Parent { get; } // will throw if no applicable parent exists (e.g. variable access)
        StackEntry Result { set; }
    }

    internal interface ITranspilerLibraryMapper
    {
        GenerateCallDelegate? TryMapMethod(MethodReference methodRef) { return null; }
        IMappedFromCILType? TryMapType(TypeReference ilTypeRef) { return null; }
        IValueBehaviour? TryMapFieldBehavior(FieldReference fieldRef) { return null; }
    }

    internal interface IITranspilerLibraryScanner
    {
        StorageClass? TryScanStorageClass(ICustomAttributeProvider fieldDef);
        IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider fieldDef);
    }

    internal delegate IEnumerable<SpirvInstruction> GenerateCallDelegate(ITranspilerMethodContext context);

    internal interface IMappedFromCILType { }

    internal class TranspilerLibrary 
    {
        private readonly TypeDefinition ilModuleType;
        private readonly Module module;
        private readonly Action<DefinedFunction, MethodBody> queueMethodBody;
        private readonly Dictionary<string, GenerateCallDelegate> mappedMethods = new Dictionary<string, GenerateCallDelegate>();
        private readonly Dictionary<string, IMappedFromCILType> mappedTypes = new Dictionary<string, IMappedFromCILType>();
        private readonly Dictionary<string, IValueBehaviour> mappedFields = new Dictionary<string, IValueBehaviour>();
        private readonly Dictionary<string, IValueBehaviour> mappedParameters = new Dictionary<string, IValueBehaviour>();
        private readonly Dictionary<string, Function> mappedFunctions = new Dictionary<string, Function>();
        private readonly StructMapper structMapper;
        private readonly ReferenceMapper referenceMapper;
        private readonly InternalMethodMapper methodMapper;

        public TranspilerOptions Options { get; set; } = new TranspilerOptions();

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

        public TranspilerLibrary(TypeDefinition ilModuleType, Module module, Action<DefinedFunction, MethodBody> queueMethodBody)
        {
            this.ilModuleType = ilModuleType;
            this.module = module;
            this.queueMethodBody = queueMethodBody;
            structMapper = new StructMapper(this, module);
            referenceMapper = new ReferenceMapper(this);
            methodMapper = new InternalMethodMapper(this);
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

        public IValueBehaviour MapField(FieldReference fieldRef, object? parentTag)
        {
            string mappingName = fieldRef.FullName;
            if (parentTag is VarGroup parentVarGroup) // VarGroups may be instanced so FullName is not unique
                mappingName += "@" + parentVarGroup.Name;
            if (mappedFields.TryGetValue(mappingName, out var mapped))
                return mapped;

            mapped = MapType(fieldRef.FieldType) switch
            {
                VarGroup varGroup => varGroup,
                VarGroupTemplate template => InstantiateTemplateFor(fieldRef.FullName, fieldRef.Resolve(), template),
                SpirvType realType when (fieldRef.DeclaringType.FullName == ilModuleType.FullName) => ScanAndMapGlobalVariable(realType),

                SpirvType realType => MapType(fieldRef.DeclaringType) switch
                {
                    SpirvStructType declaringStructType => declaringStructType.Members.First(m => m.Name == fieldRef.Name),
                    VarGroup varGroup => varGroup.Variables.First(v => v.Name == fieldRef.FullName),
                    VarGroupTemplate => (parentTag as VarGroup ??
                        throw new InvalidOperationException("Somehow a field is mapped from a variable group template without an instance of that template"))
                        .Variables.First(v => v.Name == fieldRef.FullName),

                    _ => TryMapFieldBehavior(fieldRef) ?? throw new NotSupportedException("Unsupported field container type")
                },

                _ => throw new NotSupportedException("Unsupported field type")
            };
            mappedFields[mappingName] = mapped;
            return mapped;

            Variable ScanAndMapGlobalVariable(SpirvType realType)
            {
                var storageClass = TryScanStorageClass(fieldRef.Resolve())
                    ?? throw new InvalidOperationException($"Could not scan storage class for field {fieldRef.FullName}");
                var decorations = ScanDecorations(fieldRef.Resolve());
                return CreateGlobalVariable(realType, fieldRef.Name, storageClass, decorations);
            }
        }

        private VarGroup InstantiateTemplateFor(string elementName, ICustomAttributeProvider element, VarGroupTemplate template)
        {
            var storageClass = TryScanStorageClass(element);
            if (storageClass == null)
                throw new InvalidOperationException("Field has to have a storage class to instantiate a var group template");

            return structMapper.MapVarGroup($"{elementName}#VarGroup", template.TypeDefinition, storageClass);
        }

        private IValueBehaviour? TryMapFieldBehavior(FieldReference fieldRef) => AllMappers
             .Select(mapper => mapper.TryMapFieldBehavior(fieldRef))
             .FirstOrDefault(b => b != null);

        private IEnumerable<DecorationEntry> ScanDecorations(ICustomAttributeProvider element) => AllScanners
            .Select(scanner => scanner.TryScanDecorations(element))
            .FirstOrDefault(c => c.Any())
            ?? Enumerable.Empty<DecorationEntry>();

        private StorageClass? TryScanStorageClass(ICustomAttributeProvider element) => AllScanners
            .Select(scanner => scanner.TryScanStorageClass(element))
            .FirstOrDefault(c => c.HasValue);

        public IValueBehaviour MapParameter(ParameterDefinition paramDef, Function function)
        {
            var mappingName = $"{function.Name}#{paramDef.Name}";
            if (mappedParameters.TryGetValue(mappingName, out var mapped))
                return mapped;

            var paramType = MapType(paramDef.ParameterType);
            var storageClass = TryScanStorageClass(paramDef);
            var decorations = ScanDecorations(paramDef);

            mapped = paramType switch
            {
                VarGroup varGroup => varGroup,
                VarGroupTemplate template => InstantiateTemplateFor(mappingName, paramDef, template),
                SpirvType realType when storageClass != null => CreateGlobalVariable(realType, paramDef.Name, storageClass.Value, decorations),
                SpirvType realType => MapSpirvParameter(realType),
                MappedFromRefCILType byRef => byRef.ElementType switch
                {
                    VarGroup varGroup => varGroup, // ignore by-ref for VarGroup(Template)
                    VarGroupTemplate template => InstantiateTemplateFor(mappingName, paramDef, template),
                    SpirvType realType when storageClass.HasValue => new GlobalVariableReference(CreateGlobalVariable(realType, paramDef.Name, storageClass.Value, decorations)),
                    _ => throw new NotSupportedException("Unsupported by-reference parameter type")
                },
                _ => throw new NotSupportedException("Unsupported parameter type")
            };
            mappedParameters[mappingName] = mapped;
            return mapped;

            Parameter MapSpirvParameter(SpirvType realType)
            {
                if (function is EntryFunction)
                    throw new InvalidOperationException("Entry point parameters require a storage class");
                var parameter = new Parameter(function.Parameters.Count, paramDef.Name, realType)
                {
                    Decorations = decorations.ToHashSet()
                };
                function.Parameters.Add(parameter);
                return parameter;
            }
        }

        private bool IsBlockStructure(SpirvType type) =>
            type is SpirvStructType && type.Decorations.Any(d => d.Kind == Decoration.Block);

        private GlobalVariable CreateGlobalVariable(SpirvType realType, string name, StorageClass storageClass, IEnumerable<DecorationEntry>? decorations = null)
        {
            decorations ??= Enumerable.Empty<DecorationEntry>();

            if (Options.ImplicitUniformBlockStructures && storageClass == StorageClass.Uniform && !IsBlockStructure(realType))
                return CreateImplicitBlockVariable(realType, name, decorations, byRef: false);

            var variable = new GlobalVariable(name, MakePointer(realType))
            {
                Decorations = decorations.ToHashSet()
            };
            module.GlobalVariables.Add(variable);
            return variable;

            SpirvPointerType MakePointer(SpirvType elementType) => new SpirvPointerType()
            {
                Type = elementType,
                StorageClass = storageClass
            };
        }

        private ImplicitBlockVariable CreateImplicitBlockVariable(SpirvType realType, string name, IEnumerable<DecorationEntry> decorations, bool byRef)
        {
            var implicitStruct = new SpirvStructType()
            {
                Members = new[]
                {
                    new SpirvMember(0, name, realType, new[] { Decorations.Offset(0) }.ToHashSet())
                }.ToImmutableArray(),
                Decorations = new[]
                {
                    Decorations.Block()
                }.ToHashSet()
            };
            var variable = new ImplicitBlockVariable(name, implicitStruct, realType, decorations, byRef);
            module.GlobalVariables.Add(variable);
            return variable;
        }

        private (SpirvType, IValueBehaviour?) MapReturnValue(MethodDefinition ilMethod, bool isEntryPoint)
        {
            var mappedType = MapType(ilMethod.ReturnType);
            var storageClass = TryScanStorageClass(ilMethod.MethodReturnType);
            var isActualValue = mappedType is SpirvType && storageClass == null;

            var returnType = isActualValue ? mappedType as SpirvType : new SpirvVoidType();
            if (isEntryPoint && isActualValue && mappedType is not SpirvVoidType)
                throw new InvalidOperationException("Entry-point functions can only return global variables or variable groups");
            if (!isEntryPoint && !isActualValue)
                throw new InvalidOperationException("Non-entry point functions can only return SPIRV values");

            IValueBehaviour? behaviour = mappedType switch
            {
                SpirvVoidType => null,
                VarGroup => null,
                VarGroupTemplate => null,
                SpirvType realType when storageClass != null => new VariableReturn(CreateGlobalVariable(realType, "#return", storageClass.Value)),
                SpirvType realType => new ValueReturn(realType),
                MappedFromRefCILType => throw new NotSupportedException("By-ref return types are not supported"),
                _ => throw new NotSupportedException("Unsupported return type")
            };

            return (returnType!, behaviour);
        }

        public Function? TryMapInternalMethod(MethodDefinition ilMethod, bool isEntryPoint)
        {
            var mappingName = ilMethod.FullName;
            if (mappedFunctions.TryGetValue(mappingName, out var mapped))
                return mapped;

            var (returnType, returnValue) = MapReturnValue(ilMethod, isEntryPoint);

            var function = 0 switch
            {
                _ when isEntryPoint && ilMethod.HasBody => new EntryFunction(ilMethod.Name, ExtractExecutionModel(ilMethod)),
                _ when isEntryPoint && !ilMethod.HasBody => throw new InvalidOperationException("An entry point method has to have a body"),
                _ when !isEntryPoint && ilMethod.HasBody => new DefinedFunction(ilMethod.Name, returnType),
                _ when !isEntryPoint && !ilMethod.HasBody => new Function(ilMethod.Name, returnType),
                _ => throw new NotImplementedException("Unexpected branch")
            };
            function.ReturnValue = returnValue;

            if (ilMethod.HasThis && ilMethod.DeclaringType.FullName != ilModuleType.FullName)
                throw new NotSupportedException("Real instance methods are not supported yet");

            foreach (var parameter in ilMethod.Parameters)
                MapParameter(parameter, function);

            mappedFunctions.Add(mappingName, function);
            module.Functions.Add(function);
            if (function is DefinedFunction definedFunction)
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
