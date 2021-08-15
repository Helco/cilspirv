using System;
using cilspirv.Spirv;

namespace cilspirv.Library
{
    public abstract class StorageClassAttributeBase : Attribute
    {
        public abstract StorageClass StorageClass { get; }
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class OutputAttribute : StorageClassAttributeBase
    {
        public override StorageClass StorageClass => StorageClass.Output;
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class UniformAttribute : StorageClassAttributeBase
    {
        public override StorageClass StorageClass => StorageClass.Uniform;
    }
}
