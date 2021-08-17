using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    public record SpirvStructType : SpirvAggregateType, IDecoratableInstructionGeneratable
    {
        public ImmutableArray<SpirvMember> Members { get; init; }

        public override string ToString() => $"{{{string.Join(", ", Members.Select(m => m.Type))}}}";

        public override IEnumerable<SpirvType> Dependencies => Members.SelectMany(m => m.Type.Dependencies.Prepend(m.Type));

        public virtual bool Equals(SpirvStructType? other) =>
            base.Equals(other) &&
            Members.ValueEquals(other.Members);

        public override int GetHashCode() =>
            Members.Aggregate(base.GetHashCode(), HashCode.Combine);

        internal override IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpTypeStruct()
            {
                Result = context.CreateIDFor(this),
                Members = Members
                    .Select(m => context.IDOf(m.Type))
                    .ToImmutableArray()
            };
        }

        IEnumerable<Instruction> IDecoratableInstructionGeneratable.GenerateDecorations(IInstructionGeneratorContext context) =>
            (this as IDecoratableInstructionGeneratable).BaseGenerateDecorations(context).Concat(
                Members.SelectMany((member, memberI) =>
                    member.GenerateDecorations(context, context.IDOf(this), memberI)));

        internal override IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context)
        {
            var structID = context.IDOf(this);
            if (Name != null)
                yield return new OpName()
                {
                    Target = structID,
                    Name = Name
                };

            var memberInstructions = Members.Select((member, memberI) => new OpMemberName()
            {
                Type = structID,
                Member = memberI,
                Name = member.Name
            });
            foreach (var instr in memberInstructions)
                yield return instr;
        }
    }

    public record SpirvMember : IDecoratable, IMappedFromCILField
    {
        public int Index { get; }
        public string Name { get; }
        public SpirvType Type { get; }
        public IReadOnlySet<DecorationEntry> Decorations { get; }

        public SpirvMember(int index, string name, SpirvType type, IReadOnlySet<DecorationEntry>? decorations)
        {
            Index = index;
            Name = name;
            Type = type;
            Decorations = decorations ?? new HashSet<DecorationEntry>();
        }

        public virtual bool Equals(SpirvMember? other) =>
            other != null &&
            EqualityContract == other.EqualityContract &&
            Index == other.Index &&
            Name == other.Name &&
            Type == other.Type &&
            Decorations.SetEquals(other.Decorations);

        public override int GetHashCode() => Decorations.Aggregate(
            HashCode.Combine(EqualityContract, Index, Name, Type),
            HashCode.Combine);

        internal IEnumerable<Instruction> GenerateDecorations(IInstructionGeneratorContext context, ID structID, int memberI)
        {
            foreach (var entry in Decorations)
            {
                var stringOperands = entry.ExtraOperands.Where(o => o.Kind == ExtraOperandKind.String).ToImmutableArray();
                var idOperands = entry.ExtraOperands.Where(o => o.Kind == ExtraOperandKind.ID).ToImmutableArray();
                var numericOperands = entry.ExtraOperands.Except(stringOperands.Concat(idOperands)).ToImmutableArray();
                if (idOperands.Any())
                    throw new InvalidOperationException("Cannot use ID operands for struct member decorations");

                yield return new OpMemberDecorate()
                {
                    StructureType = structID,
                    Member = memberI,
                    Decoration = entry.Kind,
                    ExtraOperands = numericOperands
                };

                if (stringOperands.Any())
                {
                    yield return new OpMemberDecorateString()
                    {
                        StructType = structID,
                        Member = memberI,
                        Decoration = entry.Kind,
                        ExtraOperands = stringOperands
                    };
                }
            }
        }
    }
}
