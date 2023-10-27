using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Values;

namespace cilspirv.Transpiler
{
    internal record SpirvStructType : SpirvAggregateType, IDecoratableInstructionGeneratable
    {
        public ImmutableArray<StructMember> Members { get; init; }

        public override string ToString() => $"{{{string.Join(", ", Members.Select(m => m.Type))}}}";

        public override IEnumerable<SpirvType> Dependencies => Members.SelectMany(m => m.Type.Dependencies.Prepend(m.Type));

        public virtual bool Equals(SpirvStructType? other) =>
            base.Equals(other) &&
            Members.ValueEquals(other.Members);

        public override int GetHashCode() =>
            Members.Aggregate(base.GetHashCode(), HashCode.Combine);

        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeStruct()
            {
                Result = context.CreateIDFor(this),
                Members = Members
                    .Select(m => context.IDOf(m.Type))
                    .ToImmutableArray()
            };
        }

        IEnumerable<Instruction> IDecoratableInstructionGeneratable.GenerateDecorations(IIDMapper context) =>
            (this as IDecoratableInstructionGeneratable).BaseGenerateDecorations(context).Concat(
                Members.SelectMany((member, memberI) =>
                    member.GenerateDecorations(context, context.IDOf(this), memberI)));

        internal override IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context)
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
}
