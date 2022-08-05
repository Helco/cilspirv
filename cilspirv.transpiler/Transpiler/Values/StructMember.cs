using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler.Values
{
    public record StructMember :
        IDecoratable,
        IValueBehaviour
    {
        public int Index { get; }
        public string Name { get; }
        public SpirvType Type { get; }
        public IReadOnlySet<DecorationEntry> Decorations { get; }

        public StructMember(int index, string name, SpirvType type, IReadOnlySet<DecorationEntry>? decorations)
        {
            Index = index;
            Name = name;
            Type = type;
            Decorations = decorations ?? new HashSet<DecorationEntry>();
        }

        public virtual bool Equals(StructMember? other) =>
            other != null &&
            EqualityContract == other.EqualityContract &&
            Index == other.Index &&
            Name == other.Name &&
            Type == other.Type &&
            Decorations.SetEquals(other.Decorations);

        public override int GetHashCode() => Decorations.Aggregate(
            HashCode.Combine(EqualityContract, Index, Name, Type),
            HashCode.Combine);

        internal IEnumerable<Instruction> GenerateDecorations(IIDMapper context, ID structID, int memberI)
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

        private (OpAccessChain instruction, SpirvPointerType pointerType) CreateAccessChain(ITranspilerValueContext context)
        {
            if (context.Parent is not ValueStackEntry parentValue)
                throw new InvalidOperationException("Struct member parent is not a value");
            if (parentValue.Type is not SpirvPointerType parentPointerType)
                throw new InvalidOperationException("Struct member parent is not a pointer, cannot load address");

            var resultType = new SpirvPointerType()
            {
                Type = Type,
                StorageClass = parentPointerType.StorageClass
            };
            return (new OpAccessChain()
            {
                Result = context.CreateID(),
                ResultType = context.IDOf(resultType),
                Base = parentValue.ID,
                Indexes = ImmutableArray.Create(
                    context.IDOf(
                        new NumericConstant(Index)))
            }, resultType);
        }

        IEnumerable<Instruction> IValueBehaviour.LoadAddress(ITranspilerValueContext context)
        {
            var (instruction, resultType) = CreateAccessChain(context);
            context.Result = new ValueStackEntry(this, instruction.Result, resultType);
            yield return instruction;
        }

        IEnumerable<Instruction> IValueBehaviour.Load(ITranspilerValueContext context)
        {
            if (context.Parent is not ValueStackEntry parentValue)
                throw new InvalidOperationException("Struct member parent is not a value");
            var result = new ValueStackEntry(this, context.CreateID(), Type);
            context.Result = result;

            if (parentValue.Type is SpirvPointerType)
            {
                var (accessChain, _) = CreateAccessChain(context);
                yield return accessChain;
                yield return new OpLoad()
                {
                    Result = result.ID,
                    ResultType = context.IDOf(Type),
                    Pointer = accessChain.Result
                };
            }
            else
            {
                yield return new OpCompositeExtract()
                {
                    Result = result.ID,
                    ResultType = context.IDOf(Type),
                    Composite = parentValue.ID,
                    Indexes = ImmutableArray.Create<LiteralNumber>(Index)
                };
            }
        }
    }
}
