using System;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    public sealed record TranspilerOptions
    {
        public bool DebugInfo { get; init; } = true;
        public bool DefaultTypeUserNames { get; init; } = false;
        public bool SkipUnusedFunctions { get; init; } = true;
        public int NativeIntWidth { get; init; } = 32;
        public SelectionControl DefaultSelectionControl { get; init; } = SelectionControl.Flatten;
    }
}
