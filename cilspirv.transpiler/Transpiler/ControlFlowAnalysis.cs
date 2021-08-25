using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace cilspirv.Transpiler
{
    internal enum ControlFlowKind
    {
        None = 0,
        Selection,
        Loop
    }

    internal interface IControlFlowBlock
    {
        ControlFlowKind ControlFlowKind { get; }
        IEnumerable<Instruction> Instructions { get; }
        int InboundStackSize { get; }
    }

    internal class ControlFlowAnalysis
    {
        internal enum BlockKind
        {
            Normal,
            HeaderBlock,
            MergeBlock
        }

        internal enum ExitKind
        {
            None,
            Branch,
            CondBranch,
            Exit // any kind, e.g. return, kill, unreachable
        }

        internal class Block : IControlFlowBlock
        {
            public BlockKind BlockKind { get; set; }
            public ControlFlowKind ControlFlowKind { get; set; }
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
            public bool Dominates(Block b)
            {
                var curB = b;
                while (curB != null && curB != this && curB != curB.ImmediateDominator)
                    curB = curB.ImmediateDominator;
                return curB == this;
            }
        }

        private readonly MethodBody ilMethodBody;
        private readonly List<Block> allBlocks = new List<Block>();
        private readonly Dictionary<int, Block> blocksByOffset = new Dictionary<int, Block>();

        public IReadOnlyList<IControlFlowBlock> Blocks => allBlocks;

        public ControlFlowAnalysis(MethodBody ilMethodBody)
        {
            this.ilMethodBody = ilMethodBody;
        }

        public void Analyse()
        {
            CreateInitialBlocks();
            SetOutboundEdges();
            SetInboundEdges();
            SetPostOrderNumbers();
            DetermineDominators();
        }

        /// <summary>Generates instruction blocks without any graph or flow info</summary>
        private void CreateInitialBlocks()
        {
            int blockStart = -1;
            for (int i = 0; i < ilMethodBody.Instructions.Count; i++)
            {
                if (blockStart < 0)
                    blockStart = i;
                var ilInstr = ilMethodBody.Instructions[i];

                var exitKind = ilInstr.OpCode.FlowControl switch
                {
                    FlowControl.Branch => ExitKind.Branch,
                    FlowControl.Cond_Branch => ExitKind.CondBranch,
                    FlowControl.Return => ExitKind.Exit,
                    FlowControl.Throw => ExitKind.Exit,
                    _ => ExitKind.None
                };
                if (exitKind != ExitKind.None)
                    FinishBlock(i, exitKind);
            }
            if (blockStart >= 0)
                FinishBlock(ilMethodBody.Instructions.Count - 1, ExitKind.None);

            void FinishBlock(int lastInstrI, ExitKind exitKind)
            {
                var block = new Block()
                {
                    Instructions = ilMethodBody.Instructions.Skip(blockStart).Take(lastInstrI - blockStart + 1),
                    ExitKind = exitKind
                };
                allBlocks.Add(block);
                blocksByOffset.Add(ilMethodBody.Instructions[blockStart].Offset, block);
                blockStart = -1;
            }
        }

        private void SetOutboundEdges()
        {
            var edges = new List<(Block, Instruction)>(allBlocks.Count);
            foreach (var block in allBlocks)
            {
                var lastInstr = block.Instructions.Last();
                if (lastInstr.Operand is Instruction branchInstr)
                    edges.Add((block, branchInstr));
                if (lastInstr.OpCode.Code != Code.Ret &&
                    lastInstr.OpCode.Code != Code.Throw &&
                    lastInstr.OpCode.Code != Code.Rethrow)
                    edges.Add((block, lastInstr.Next));
            }

            foreach (var (from, toInstr) in edges)
            {
                if (blocksByOffset.TryGetValue(toInstr.Offset, out var to))
                {
                    from.OutboundEdges.Add(to);
                    continue;
                }

                var containingBlockI = allBlocks.FindIndex(b => b.Instructions.Contains(toInstr));
                if (containingBlockI < 0)
                    throw new InvalidOperationException("No block contains target instruction");

                var containingBlock = allBlocks[containingBlockI];
                var prefixBlock = new Block()
                {
                    Instructions = containingBlock.Instructions.TakeWhile(i => i != toInstr)
                        .Append(Instruction.Create(OpCodes.Br, toInstr)),
                    ExitKind = ExitKind.Branch
                };
                containingBlock.Instructions = containingBlock.Instructions.SkipWhile(i => i != toInstr);
                allBlocks.Insert(containingBlockI, prefixBlock);
                blocksByOffset[prefixBlock.Instructions.First().Offset] = prefixBlock;
                blocksByOffset[toInstr.Offset] = containingBlock;

                from.OutboundEdges.Add(containingBlock);
                prefixBlock.OutboundEdges.Add(containingBlock);
            }
        }

        private void SetInboundEdges()
        {
            var blockIndices = allBlocks
                .Select((block, i) => (block, i))
                .ToDictionary(t => t.block, t => t.i);
            for (int fromI = 0; fromI < allBlocks.Count; fromI++)
            {
                var from = allBlocks[fromI];
                foreach (var to in from.OutboundEdges)
                {
                    // easy backedge detection thanks to CIL restrictions
                    //

                    int toI = blockIndices[to];
                    (fromI < toI
                        ? to.InboundForwardEdges
                        : to.InboundBackwardEdges)
                        .Add(from);
                }
            }
        }
    
        private void SetPostOrderNumbers()
        {
            var stack = new Stack<(Block, int)>();
            stack.Push((allBlocks[0], 0));
            int next = 0;
            while (stack.Any())
            {
                var (parent, edgeI) = stack.Pop();
                if (edgeI < parent.OutboundEdges.Count)
                {
                    var child = parent.OutboundEdges[edgeI];
                    stack.Push((parent, ++edgeI));
                    if (child.PostOrderI < 0)
                        stack.Push((child, 0));
                }
                else
                    parent.PostOrderI = next++;
            }
        }

        private void DetermineDominators()
        {
            // Cooper, Harvey, Kennedy - "A Simple, Fast Dominance Algorithm"
            var revPostOrderBlocks = allBlocks
                .OrderByDescending(b => b.PostOrderI)
                .Skip(1) // the first is the root which has no predecessors
                .ToArray();
            allBlocks[0].ImmediateDominator = allBlocks[0];

            bool changed;
            do
            {
                changed = false;
                foreach (var block in revPostOrderBlocks)
                {
                    var inboundEdges = block.InboundForwardEdges
                        .Cast<Block?>()
                        .Concat(block.InboundBackwardEdges)
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

            static Block? Intersect(Block? b1, Block? b2)
            {
                while (b1 != b2)
                {
                    while (b1?.PostOrderI < b2?.PostOrderI)
                        b1 = b1?.ImmediateDominator;
                    while (b2?.PostOrderI < b1?.PostOrderI)
                        b2 = b2?.ImmediateDominator;
                }
                return b1;
            }
        }
    }
}
