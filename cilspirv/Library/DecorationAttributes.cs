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

    public class LocationAttribute : DecorationAttribute
    {
        public LocationAttribute(uint location) : base(new[]
        {
            Location(location)
        })
        { }
    }

    public class BindingAttribute : DecorationAttribute
    {
        public BindingAttribute(uint set, uint binding) : base(new[]
        {
            Binding(binding),
            DescriptorSet(set)
        })
        { }
    }
}
