using System;
using cilspirv.Spirv;

namespace cilspirv.Library
{
    public abstract class ModuleAttributeBase : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EntryPointAttribute : Attribute
    {
        public ExecutionModel ExecutionModel { get; }

        public EntryPointAttribute(ExecutionModel executionModel) =>
            (ExecutionModel) = executionModel;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CapabilityAttribute : ModuleAttributeBase
    {
        public Capability[] Capabilities { get; }

        public CapabilityAttribute(params Capability[] capabilities) =>
            Capabilities = capabilities;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExtensionAttribute : ModuleAttributeBase
    {
        public string[] Extensions{ get; }

        public ExtensionAttribute(params string[] extensions) =>
            Extensions = extensions;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class MemoryModelAttribute : ModuleAttributeBase
    {
        public AddressingModel AddressingModel { get; }
        public MemoryModel MemoryModel { get; }

        public MemoryModelAttribute(AddressingModel addressingModel, MemoryModel memoryModel) =>
            (AddressingModel, MemoryModel) = (addressingModel, memoryModel);
    }
}
