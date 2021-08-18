using System;
using cilspirv.Spirv;
using static cilspirv.Spirv.Decorations;

namespace cilspirv.Library
{
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BuiltInAttribute : DecorationAttribute
    {
        public BuiltInAttribute(BuiltIn builtIn) : base(new[]
        {
            BuiltIn(builtIn)
        })
        { }
    }
}
