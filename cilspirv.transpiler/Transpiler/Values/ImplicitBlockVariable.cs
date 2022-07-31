using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler.Values
{
    internal class ImplicitBlockVariable : GlobalVariable, IValueBehaviour
    {
        public string BlockName { get; }
        public SpirvType BlockType { get; }
        public SpirvPointerType BlockPointerType { get; }

        public ImplicitBlockVariable(string actualName, SpirvType blockType, SpirvType actualType, IEnumerable<DecorationEntry> decorations, bool byRef)
            : base(actualName, byRef ? MakePointerType(MakePointerType(actualType)) : MakePointerType(actualType))
        {
            BlockName = $"{actualName}#Block";
            BlockType = blockType;
            BlockPointerType = MakePointerType(BlockType, decorations);
        }

        private static SpirvPointerType MakePointerType(SpirvType baseType, IEnumerable<DecorationEntry>? decorations = null) => new SpirvPointerType()
        {
            Type = baseType,
            StorageClass = StorageClass.Uniform,
            Decorations = decorations?.ToHashSet() ?? new HashSet<DecorationEntry>()
        };

        public override IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpVariable()
            {
                Result = context.CreateIDFor(this),
                ResultType = context.IDOf(BlockPointerType),
                StorageClass = StorageClass
            };
        }

        public override IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context)
        {
            if (!string.IsNullOrEmpty(BlockName))
                yield return new OpName()
                {
                    Target = context.IDOf(this),
                    Name = BlockName
                };
        }

        IEnumerable<Instruction> IValueBehaviour.LoadAddress(ITranspilerValueContext context)
        {
            MarkUsageIn(context.Function);
            var resultId = context.CreateID();
            context.Result = new ValueStackEntry(this, resultId, PointerType);
            yield return new OpAccessChain()
            {
                Result = resultId,
                ResultType = context.IDOf(PointerType),
                Base = context.IDOf(this),
                Indexes = new[]
                {
                    context.IDOf(new NumericConstant(0))
                }.ToImmutableArray()
            };
        }
    }
}