using System;
using cilspirv.Spirv;

namespace cilspirv.Library
{
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = false)]
    public class StorageClassAttribute : Attribute
    {
        public virtual StorageClass StorageClass { get; init; }
    }
    
    public sealed class OutputAttribute : StorageClassAttribute
    {
        public override StorageClass StorageClass => StorageClass.Output;
    }

    public sealed class InputAttribute : StorageClassAttribute
    {
        public override StorageClass StorageClass => StorageClass.Input;
    }

    public sealed class UniformAttribute : StorageClassAttribute
    {
        public override StorageClass StorageClass => StorageClass.Uniform;
    }
}
