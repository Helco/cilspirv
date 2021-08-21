using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler;

namespace cilspirv.Library
{
    public sealed record StandardFieldBehavior : ITranspilerFieldBehavior
    {
        private readonly SpirvType type;
        private readonly int memberIndex;

        public StandardFieldBehavior(SpirvType type, int memberIndex) =>
            (this.type, this.memberIndex) = (type, memberIndex);

        IEnumerable<Instruction> ITranspilerFieldBehavior.LoadAddress(ITranspilerFieldContext context)
        {
            if (context.Parent is not ValueStackEntry parentValue)
                throw new InvalidOperationException("Struct member parent is not a value");
            if (parentValue.Type is not SpirvPointerType parentPointerType)
                throw new InvalidOperationException("Struct member parent is not a pointer");

            var resultType = new SpirvPointerType()
            {
                Type = type,
                StorageClass = parentPointerType.StorageClass
            };
            var result = new ValueStackEntry(this, context.CreateID(), resultType);
            context.Result = result;
            yield return new OpAccessChain()
            {
                Result = result.ID,
                ResultType = context.IDOf(resultType),
                Base = parentValue.ID,
                Indexes = ImmutableArray.Create(
                    context.IDOf(
                        new TranspilerNumericConstant(memberIndex)))
            };
        }
    }
}
