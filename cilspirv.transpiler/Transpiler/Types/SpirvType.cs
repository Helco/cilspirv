using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler;

namespace cilspirv.Transpiler
{
    public abstract record SpirvType : IEquatable<SpirvType>,
        IDecoratableInstructionGeneratable,
        IDebugInstructionGeneratable,
        IMappedFromCILType
    {
        public string Name => UserName ?? ToString();
        public string? UserName { get; init; }
        public IReadOnlySet<DecorationEntry> Decorations { get; init; } = new HashSet<DecorationEntry>();

        public virtual IEnumerable<SpirvType> Dependencies => Enumerable.Empty<SpirvType>();

        public virtual bool Equals(SpirvType? other) =>
            other is not null &&
            EqualityContract == other.EqualityContract &&
            (this is not SpirvAggregateType || Name == other.Name) &&
            Decorations.SetEquals(other.Decorations);

        public override int GetHashCode() => Decorations.Aggregate(
            HashCode.Combine(
                this is SpirvAggregateType ? Name : "",
                EqualityContract),
            HashCode.Combine);

        IEnumerator<Instruction> IInstructionGeneratable.GenerateInstructions(IIDMapper context) => GenerateInstructions(context);
        internal abstract IEnumerator<Instruction> GenerateInstructions(IIDMapper context);

        IEnumerator<Instruction> IDebugInstructionGeneratable.GenerateDebugInfo(IIDMapper context) => GenerateDebugInfo(context);
        internal virtual IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context)
        {
            var relevantName = context.Options.DefaultTypeUserNames ? Name : UserName;
            if (relevantName != null)
                yield return new OpName()
                {
                    Target = context.IDOf(this),
                    Name = relevantName
                };
        }
    }

    public abstract record SpirvOpaqueType : SpirvType { }
    public abstract record SpirvScalarType : SpirvType { }
    public abstract record SpirvNumericType : SpirvScalarType { }
    public abstract record SpirvCompositeType : SpirvType { }
    public abstract record SpirvAggregateType : SpirvCompositeType { }

    public sealed record SpirvVoidType : SpirvType
    {
        public override string ToString() => "Void";
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeVoid() { Result = context.CreateIDFor(this) };
        }
    }

    public record SpirvPointerType : SpirvType
    {
        public SpirvType? Type { get; init; }
        public StorageClass StorageClass { get; init; }
        public override string ToString() => $"Ptr<{Type}>({StorageClass})";
        public override IEnumerable<SpirvType> Dependencies => new[] { Type! }.Concat(Type!.Dependencies);
        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypePointer()
            {
                Result = context.CreateIDFor(this),
                Type = context.IDOf(Type ?? throw new InvalidOperationException("Type is not set")),
                StorageClass = StorageClass
            };
        }
    }

    public record SpirvFunctionType : SpirvType
    {
        public SpirvType? ReturnType { get; init; }
        public ImmutableArray<SpirvType> ParameterTypes { get; init; }
        public override string ToString() => $"{ReturnType}({string.Join(", ", ParameterTypes)})";

        public override IEnumerable<SpirvType> Dependencies =>
            new[] { ReturnType! }
            .Concat(ReturnType!.Dependencies)
            .Concat(ParameterTypes)
            .Concat(ParameterTypes.SelectMany(p => p.Dependencies));

        public virtual bool Equals(SpirvFunctionType? other) =>
            base.Equals(other) &&
            ReturnType == other.ReturnType &&
            ParameterTypes.ValueEquals(other.ParameterTypes);

        public override int GetHashCode() => HashCode.Combine(
            base.GetHashCode(), ReturnType,
            ParameterTypes.IsDefaultOrEmpty ? 0 : ParameterTypes.Aggregate(0, HashCode.Combine));

        internal override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpTypeFunction()
            {
                Result = context.CreateIDFor(this),
                ReturnType = context.IDOf(ReturnType ?? throw new InvalidOperationException("ReturnType is not set")),
                Parameters = ParameterTypes.Select(context.IDOf).ToImmutableArray()
            };
        }
    }
}
