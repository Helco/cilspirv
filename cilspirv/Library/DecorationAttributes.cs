using System;
using System.Collections.Generic;
using cilspirv.Spirv;

using static cilspirv.Spirv.Decorations;

namespace cilspirv.Library
{
    public abstract class DecorationAttributeBase : Attribute
    {
        public DecorationEntry[] Decorations { get; }

        protected DecorationAttributeBase(DecorationEntry[] decorations) => Decorations = decorations;
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class LocationAttribute : DecorationAttributeBase
    {
        public LocationAttribute(uint location) : base(new[]
        {
            Location(location)
        })
        { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BindingAttribute : DecorationAttributeBase
    {
        public BindingAttribute(uint binding, uint set = 0) : base(new[]
        {
            Binding(binding),
            DescriptorSet(set)
        })
        { }
    }
}
