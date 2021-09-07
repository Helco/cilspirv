using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cilspirv.Transpiler
{
    partial class ControlFlowAnalysis
    {
        private interface IDominatorAccess
        {
            int OrderI { get; }
            IDominatorAccess? ImmediateDominator { get; set; }
            IEnumerable<IDominatorAccess> InboundEdges { get; }
        }

        private readonly struct ForwardDominatorAccess : IDominatorAccess
        {
            private readonly Block block;

            public ForwardDominatorAccess(Block block) => this.block = block;

            public int OrderI => block.PostOrderI;

            public IDominatorAccess? ImmediateDominator
            {
                get => block.ImmediateDominator == null
                    ? null
                    : new ForwardDominatorAccess(block.ImmediateDominator);
                set => block.ImmediateDominator = ((ForwardDominatorAccess)value!).block;
            }
             
            public IEnumerable<IDominatorAccess> InboundEdges => block.InboundBackwardEdges
                .Concat(block.InboundForwardEdges)
                .Select(b => new ForwardDominatorAccess(b) as IDominatorAccess);
        }

        private readonly struct BackwardDominatorAccess : IDominatorAccess
        {
            private readonly Block block;

            public BackwardDominatorAccess(Block block) => this.block = block;

            public int OrderI => block.PreOrderI;

            public IDominatorAccess? ImmediateDominator
            {
                get => block.ImmediatePostDominator == null
                    ? null
                    : new BackwardDominatorAccess(block.ImmediatePostDominator);
                set => block.ImmediatePostDominator = ((BackwardDominatorAccess)value!).block;
            }

            public IEnumerable<IDominatorAccess> InboundEdges => block.OutboundEdges
                .Select(b => new BackwardDominatorAccess(b) as IDominatorAccess);
        }

        private void DeterminePreDominators() => DetermineDominators(allBlocks.Select(b => new ForwardDominatorAccess(b) as IDominatorAccess));
        private void DeterminePostDominators() => DetermineDominators(allBlocks.Select(b => new BackwardDominatorAccess(b) as IDominatorAccess));

        /// <remarks>Cooper, Harvey, Kennedy - "A Simple, Fast Dominance Algorithm"</remarks>
        private static void DetermineDominators(IEnumerable<IDominatorAccess> allBlocks)
        {
            var revOrderBlocks = allBlocks
                .Where(b => !b.InboundEdges.Any())
                .OrderByDescending(b => b.OrderI)
                .ToArray();
            foreach (var startBlock in allBlocks.Where(b => b.InboundEdges.Any()))
                startBlock.ImmediateDominator = startBlock;

            bool changed;
            do
            {
                changed = false;
                foreach (var block in revOrderBlocks)
                {
                    var inboundEdges = block.InboundEdges
                        .Cast<IDominatorAccess?>()
                        .Where(b => b?.ImmediateDominator != null);
                    if (!inboundEdges.Any())
                        continue;
                    var newIDom = inboundEdges.Aggregate(Intersect);
                    if (newIDom != block.ImmediateDominator)
                    {
                        block.ImmediateDominator = newIDom;
                        changed = true;
                    }
                }
            } while (changed);

            static IDominatorAccess? Intersect(IDominatorAccess? b1, IDominatorAccess? b2)
            {
                while (b1 != b2)
                {
                    while (b1?.OrderI < b2?.OrderI)
                        b1 = b1?.ImmediateDominator;
                    while (b2?.OrderI < b1?.OrderI)
                        b2 = b2?.ImmediateDominator;
                }
                return b1;
            }
        }
    }
}
