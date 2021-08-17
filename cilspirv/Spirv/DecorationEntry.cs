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

        public bool Equals(DecorationEntry? other) =>
            other is not null &&
            Kind == other.Kind &&
            ExtraOperands.ValueEquals(other.ExtraOperands);

        public override int GetHashCode() =>
            ExtraOperands.Aggregate(Kind.GetHashCode(), HashCode.Combine);
    }
}
