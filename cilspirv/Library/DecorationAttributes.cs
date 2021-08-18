using System;
using System.Collections.Generic;
using cilspirv.Spirv;

using static cilspirv.Spirv.Decorations;

namespace cilspirv.Library
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DecorationAttribute : Attribute
    {
        public DecorationEntry[] Decorations { get; }

        protected DecorationAttribute(params DecorationEntry[] decorations) => Decorations = decorations;
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class LocationAttribute : DecorationAttribute
    {
        public LocationAttribute(uint location) : base(new[]
        {
            Location(location)
        })
        { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BindingAttribute : DecorationAttribute
    {
        public BindingAttribute(uint binding, uint set = 0) : base(new[]
        {
            Binding(binding),
            DescriptorSet(set)
        })
        { }
    }
}
