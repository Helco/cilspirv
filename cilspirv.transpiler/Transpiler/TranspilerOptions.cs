using System;

namespace cilspirv.Transpiler
{
    public sealed record TranspilerOptions
    {
        public bool DebugInfo { get; init; } = true;
        public bool SkipUnusedFunctions { get; init; } = true;
        public int NativeIntWidth { get; init; } = 32;
    }
}
