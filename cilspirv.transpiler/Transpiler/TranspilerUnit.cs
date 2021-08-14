using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using cilspirv.Transpiler;
using cilspirv.Spirv;
using cilspirv.Library;

namespace cilspirv.Transpiler
{
    internal class TranspilerUnit
    {
        private readonly IInstructionGeneratorContext generatorContext = new InstructionGenerator();

        public TypeDefinition ILType { get; }
        public MethodDefinition ILEntryPoint { get; }
        public TranspilerModule Module { get; } = new TranspilerModule();
        public TranspilerOptions Options { get; init; } = new TranspilerOptions();

        public TranspilerUnit(MethodDefinition entryPoint) =>
            (ILType, ILEntryPoint) = (entryPoint.DeclaringType, entryPoint);

        public void ExtractModuleAttributes()
        {
            var capabilities = ILType
                .GetCustomAttributes<CapabilityAttribute>()
                .SelectMany(attr => (CustomAttributeArgument[])attr.ConstructorArguments.Single().Value)
                .Select(arg => (Capability)arg.Value);
            foreach (var capability in capabilities)
                Module.Capabilities.Add(capability);

            var extensions = ILType
                .GetCustomAttributes<ExtensionAttribute>()
                .SelectMany(attr => (CustomAttributeArgument[])attr.ConstructorArguments.Single().Value)
                .Select(arg => (string)arg.Value);
            foreach (var extension in extensions)
                Module.Extensions.Add(extension);

            var memoryModelAttr = ILType.GetCustomAttributes<MemoryModelAttribute>().FirstOrDefault();
            if (memoryModelAttr == null)
                throw new InvalidOperationException($"Module type {ILType.FullName} does not have a memory model");
            Module.AddressingModel = (AddressingModel)memoryModelAttr.ConstructorArguments[0].Value;
            Module.MemoryModel = (MemoryModel)memoryModelAttr.ConstructorArguments[1].Value;
        }
    }
}
