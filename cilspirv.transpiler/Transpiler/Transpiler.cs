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
        private readonly IDMapper idMapper = new IDMapper();
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
                idMapper.Options = value;
                Library.Options = options;
            }
        }

        public Transpiler(TypeDefinition moduleType)
        {
            ILModuleType = moduleType;
            Library = new TranspilerLibrary(moduleType, Module, (function, body) => missingBodies.Enqueue((function, body)) );
        }

        public void WriteSpirvModule(System.IO.Stream stream, bool leaveOpen = false) => new SpirvModule()
        {
            Instructions = Module.GenerateInstructions(idMapper).ToArray(),
            Bound = idMapper.Bound,
            SpirvVersion = new Version(1, 3)
        }.Write(stream, leaveOpen, idMapper.MapFromTemporaryID);

        public TranspilerFunction TranspileEntryPoint(MethodDefinition ilMethod) =>
            Library.TryMapInternalMethod(ilMethod, isEntryPoint: true) ??
            throw new ArgumentException("Could not map entry point method");

        public void TranspileAllMethodBodies()
        {
            while(missingBodies.Any())
            {
                var (definedFunction, ilBody) = missingBodies.Dequeue();
                if (definedFunction.Blocks.Any())
                    continue; // seems like we enqueued the function twice

                TranspileVariables(definedFunction, ilBody);
                var controlFlowAnalysis = new ControlFlowAnalysis(ilBody);
                controlFlowAnalysis.Analyse();
                var genInstructions = new GenInstructions(this, definedFunction, ilBody.Method, controlFlowAnalysis.Blocks);
                genInstructions.GenerateInstructions();
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
                definedFunction.Variables.Add(new Values.LocalVariable(varName, new SpirvPointerType()
                {
                    Type = type as SpirvType ?? throw new InvalidOperationException("Local variables can only be SPIRV types"),
                    StorageClass = StorageClass.Function
                }));
            }
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
