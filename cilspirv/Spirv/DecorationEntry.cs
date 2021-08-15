using System;
using System.Collections.Immutable;
using System.Linq;

namespace cilspirv.Spirv
{
    public sealed record DecorationEntry
    {
        public Decoration Kind { get; }
        public ImmutableArray<ExtraOperand> ExtraOperands { get; }

        public DecorationEntry(Decoration kind, params ExtraOperand[] operands) =>
            (Kind, ExtraOperands) = (kind, operands.ToImmutableArray());
    }
}
