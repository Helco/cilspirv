using System;

namespace cilspirv.Transpiler
{
    public sealed record TranspilerOptions
    {
        public bool DebugInfo { get; init; } = true;
        public bool SkipUnusedFunctions { get; init; } = true;
    }
}
