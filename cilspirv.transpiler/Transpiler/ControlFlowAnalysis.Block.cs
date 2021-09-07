﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace cilspirv.Transpiler
{
    internal partial class ControlFlowAnalysis
    {
        internal enum ExitKind
        {
            None,
            Branch,
            CondBranch,
            Exit // any kind, e.g. return, kill, unreachable
        }

        [Flags]
        internal enum BlockKinds
        {
            SelectionHeader = (1 << 0),
            LoopHeader = (1 << 1),
            ContinueBlock = (1 << 2),
            MergeBlock = (1 << 3),
            SelectionSwitch = (1 << 4)
        }

        internal class Block : IControlFlowBlock
        {
            public HeaderBlockKind HeaderBlockKind =>
                BlockKinds.HasFlag(BlockKinds.SelectionHeader) ? HeaderBlockKind.Selection
                : BlockKinds.HasFlag(BlockKinds.LoopHeader) ? HeaderBlockKind.Loop
                : HeaderBlockKind.None;

            public BlockKinds BlockKinds { get; set; }
            public Block? MergeBlock { get; set; }
            public Block? ContinueBlock { get; set; }
            public ExitKind ExitKind { get; set; }
            public Block? ParentHeaderBlock { get; set; }
            public List<Block> OutboundEdges { get; } = new List<Block>();
            public List<Block> InboundForwardEdges { get; } = new List<Block>();
            public List<Block> InboundBackwardEdges { get; } = new List<Block>();
            public int OutboundStackSize { get; set; }
            public int InboundStackSize { get; set; }
            public IEnumerable<Instruction> Instructions { get; set; } = Enumerable.Empty<Instruction>();

            public int PostOrderI { get; set; } = -1;
            public Block? ImmediateDominator { get; set; }
            public IEnumerable<Block> Dominators => Chain(this, b => b.ImmediateDominator);

            public int PreOrderI { get; set; } = -1;
            public Block? ImmediatePostDominator { get; set; }
            public IEnumerable<Block> PostDominators => Chain(this, b => b.ImmediatePostDominator);

            public IEnumerable<Block> AllDescendants
            {
                get
                {
                    var visited = new HashSet<Block>() { this };
                    IEnumerable<Block> curLevel = new[] { this };
                    while (curLevel.Any())
                    {
                        var nextLevel = curLevel
                            .SelectMany(b => b.OutboundEdges)
                            .Except(visited)
                            .ToArray();
                        foreach (var b in nextLevel)
                            yield return b;

                        visited.UnionWith(nextLevel);
                        curLevel = nextLevel;
                    }
                }
            }

            private static IEnumerable<Block> Chain(Block start, Func<Block, Block?> getNext)
            {
                Block? cur = start, next = getNext(start);
                while (next != null && next != cur)
                {
                    yield return next;
                    next = getNext(cur = next);
                }
            }

            IControlFlowBlock? IControlFlowBlock.MergeBlock => MergeBlock;
            IControlFlowBlock? IControlFlowBlock.ContinueBlock => ContinueBlock;
        }
    }
}
