using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Transpiler;
using cilspirv.Spirv;
using cilspirv.Library;

namespace cilspirv.Transpiler
{
    internal partial class Transpiler
    {
        private readonly InstructionGenerator generatorContext = new InstructionGenerator();
        private readonly Queue<(TranspilerDefinedFunction function, MethodBody ilBody)> missingBodies =
            new Queue<(TranspilerDefinedFunction function, MethodBody ilBody)>();
        private TranspilerOptions options = new TranspilerOptions();

        public TypeDefinition ILModuleType { get; }
        public TranspilerModule Module { get; } = new TranspilerModule();
        public TranspilerLibrary Library { get; }
        public TranspilerOptions Options
        {
            get => options;
            init
            {
                options = value;
                generatorContext.Options = value;
            }
        }

        public Transpiler(TypeDefinition moduleType)
        {
            ILModuleType = moduleType;
            Library = new TranspilerLibrary(Module);
        }

        public void WriteSpirvModule(System.IO.Stream stream, bool leaveOpen = false) => new SpirvModule()
        {
            Instructions = Module.GenerateInstructions(generatorContext).ToArray(),
            Bound = generatorContext.Bound,
            SpirvVersion = new Version(1, 3)
        }.Write(stream, leaveOpen, generatorContext.MapFromTemporaryID);

        public TranspilerFunction TranspileEntryPoint(MethodDefinition ilMethod) =>
            TranspileMethod(ilMethod, isEntryPoint: true);

        private TranspilerFunction TranspileMethod(MethodDefinition ilMethod, bool isEntryPoint)
        {
            if (!ilMethod.HasBody)
                throw new InvalidOperationException("An entry point method has to have a body");

            var returnType = Library.MapType(ilMethod.ReturnType);
            var function =
                isEntryPoint ? new TranspilerEntryFunction(ilMethod.Name, returnType, ExtractExecutionModel(ilMethod))
                : ilMethod.HasBody ? new TranspilerDefinedFunction(ilMethod.Name, returnType)
                : new TranspilerFunction(ilMethod.Name, returnType);

            if (ilMethod.Parameters.Any())
                throw new InvalidOperationException("Parameters are not supported yet");
            foreach (var parameter in ilMethod.Parameters)
            {
                if (parameter.IsOut)
                    throw new NotSupportedException($"Out or ref parameter are not supported");
            }

            Module.Functions.Add(function);
            if (function is TranspilerDefinedFunction definedFunction)
                missingBodies.Enqueue((definedFunction, ilMethod.Body));
            return function;
        }

        public void TranspileAllMethodBodies()
        {
            while(missingBodies.Any())
            {
                var (definedFunction, ilBody) = missingBodies.Dequeue();
                TranspileVariables(definedFunction, ilBody);
                TranspileInstructions(definedFunction, ilBody);
            }
        }

        private void TranspileVariables(TranspilerDefinedFunction definedFunction, MethodBody ilBody)
        {
            int nextVarName = 0;
            foreach (var ilVariable in ilBody.Variables)
            {
                if (!ilBody.Method.DebugInformation.TryGetName(ilVariable, out var varName))
                    varName = $"v{nextVarName++}";
                var type = Library.MapType(ilVariable.VariableType);
                definedFunction.Variables.Add(new TranspilerVariable(varName, new SpirvPointerType()
                {
                    Type = type.Type,
                    StorageClass = StorageClass.Function
                }));
            }
        }

        private ExecutionModel ExtractExecutionModel(MethodDefinition ilMethod)
        {
            var attr = ilMethod.GetCustomAttributes<EntryPointAttribute>().SingleOrDefault();
            if (attr == null)
                throw new InvalidOperationException($"Entry point method {ilMethod.FullName} does not have an EntryPointAttribute");

            return (ExecutionModel)attr.ConstructorArguments.Single().Value;
        }

        public void ExtractModuleAttributes()
        {
            var capabilities = ILModuleType
                .GetCustomAttributes<CapabilityAttribute>()
                .SelectMany(attr => (CustomAttributeArgument[])attr.ConstructorArguments.Single().Value)
                .Select(arg => (Capability)arg.Value);
            foreach (var capability in capabilities)
                Module.Capabilities.Add(capability);

            var extensions = ILModuleType
                .GetCustomAttributes<ExtensionAttribute>()
                .SelectMany(attr => (CustomAttributeArgument[])attr.ConstructorArguments.Single().Value)
                .Select(arg => (string)arg.Value);
            foreach (var extension in extensions)
                Module.Extensions.Add(extension);

            var memoryModelAttr = ILModuleType.GetCustomAttributes<MemoryModelAttribute>().FirstOrDefault();
            if (memoryModelAttr == null)
                throw new InvalidOperationException($"Module type {ILModuleType.FullName} does not have a memory model");
            Module.AddressingModel = (AddressingModel)memoryModelAttr.ConstructorArguments[0].Value;
            Module.MemoryModel = (MemoryModel)memoryModelAttr.ConstructorArguments[1].Value;
        }
    }
}
