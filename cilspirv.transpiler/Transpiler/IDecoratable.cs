﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal interface IDecoratable
    {
        IReadOnlySetDecorationEntry Decorations { get; }
    }

    internal interface IDecoratableInstructionGeneratable : IInstructionGeneratable, IDecoratable
    {
        IEnumerable<Instruction> GenerateDecorations(IIDMapper context);
    }

    internal static class IDecoratableExtensions
    {
        public static IEnumerable<Instruction> BaseGenerateDecorations(this IDecoratableInstructionGeneratable thiz, IIDMapper context)
        {
            var id = context.IDOf(thiz);
            foreach (var entry in thiz.Decorations.OrderBy(e => (int)e.Kind).ThenBy(e => e.GetHashCode()))
            {
                var stringOperands = entry.ExtraOperands.Where(o => o.Kind == ExtraOperandKind.String).ToImmutableArray();
                var idOperands = entry.ExtraOperands.Where(o => o.Kind == ExtraOperandKind.ID).ToImmutableArray();
                var numericOperands = entry.ExtraOperands.Except(stringOperands.Concat(idOperands)).ToImmutableArray();

                if (numericOperands.Any() || (!idOperands.Any() && !stringOperands.Any()))
                {
                    yield return new OpDecorate()
                    {
                        Target = id,
                        Decoration = entry.Kind,
                        ExtraOperands = numericOperands
                    };
                }

                if (idOperands.Any())
                {
                    yield return new OpDecorateId()
                    {
                        Target = id,
                        Decoration = entry.Kind,
                        ExtraOperands = idOperands
                    };
                }

                if (stringOperands.Any())
                {
                    yield return new OpDecorateString()
                    {
                        Target = id,
                        Decoration = entry.Kind,
                        ExtraOperands = stringOperands
                    };
                }
            }
        }
    }
}
