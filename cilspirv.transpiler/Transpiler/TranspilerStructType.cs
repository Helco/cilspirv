using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerStructType : TranspilerType, IDecoratableInstructionGeneratable
    {
        public TranspilerStructType(string name) : base(name, null!) { }

        public override SpirvType Type => new SpirvStructType()
        {
            Members = Members.Select(m => m.Type).ToImmutableArray()
        };
        public IList<TranspilerMember> Members { get; } = new List<TranspilerMember>();

        IEnumerable<Instruction> IDecoratableInstructionGeneratable.GenerateDecorations(IInstructionGeneratorContext context) =>
            (this as IDecoratableInstructionGeneratable).BaseGenerateDecorations(context).Concat(
                Members.SelectMany((member, memberI) =>
                    member.GenerateDecorations(context, context.IDOf(this), memberI)));

        public override IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context)
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

    internal class TranspilerMember : IDecoratable
    {
        public string Name { get; }
        public SpirvType Type { get; }
        public ISet<DecorationEntry> Decorations { get; } = new HashSet<DecorationEntry>();

        public TranspilerMember(string name, SpirvType type) => (Name, Type) = (name, type);

        public IEnumerable<Instruction> GenerateDecorations(IInstructionGeneratorContext context, ID structID, int memberI)
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
