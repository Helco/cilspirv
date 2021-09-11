﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace cilspirv.Transpiler
{
    internal enum HeaderBlockKind
    {
        None = 0,
        Selection,
        Loop
    }

    internal interface IControlFlowBlock
    {
        HeaderBlockKind HeaderBlockKind { get; }
        IControlFlowBlock? MergeBlock { get; }
        IControlFlowBlock? ContinueBlock { get; }
        IEnumerable<Instruction> Instructions { get; }
        int InboundStackSize { get; }
    }

    internal partial class ControlFlowAnalysis
    {
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
            SetPostOrderNumber();
            SetPostOrderRevNumber();
            DeterminePreDominators();
            DeterminePostDominators();
            ConstructLoops();
            ConstructSelections();
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

        /// <remarks>Also splits blocks where branches jump into</remarks>
        private void SetOutboundEdges()
        {
            var edges = new List<(Block source, Instruction target)>(allBlocks.Count);
            foreach (var block in allBlocks)
            {
                var lastInstr = block.Instructions.Last();
                if (lastInstr.Operand is Instruction targetInstr)
                    edges.Add((block, targetInstr));
                if (lastInstr.Operand is IEnumerable<Instruction> targetInstrs)
                    edges.AddRange(targetInstrs.Select(t => (block, t)));
                if (lastInstr.OpCode.Code != Code.Ret &&
                    lastInstr.OpCode.Code != Code.Throw &&
                    lastInstr.OpCode.Code != Code.Rethrow &&
                    lastInstr.OpCode.Code != Code.Br &&
                    lastInstr.OpCode.Code != Code.Br_S)
                    edges.Add((block, lastInstr.Next));
            }

            var splittingEdges = edges.Where(t => !blocksByOffset.ContainsKey(t.target.Offset)).ToArray();
            foreach (var (_, toInstr) in splittingEdges)
            {
                var containingBlockI = allBlocks.FindIndex(b => b.Instructions.Contains(toInstr));
                if (containingBlockI < 0)
                    throw new InvalidOperationException("No block contains target instruction");

                var branchInstr = Instruction.Create(OpCodes.Br, toInstr); 
                branchInstr.Next = toInstr;
                branchInstr.Offset = toInstr.Offset | 0xf000;

                var containingBlock = allBlocks[containingBlockI];
                var prefixBlock = new Block()
                {
                    Instructions = containingBlock.Instructions.TakeWhile(i => i != toInstr).Append(branchInstr),
                    ExitKind = ExitKind.Branch
                };
                containingBlock.Instructions = containingBlock.Instructions.SkipWhile(i => i != toInstr);
                allBlocks.Insert(containingBlockI, prefixBlock);
                blocksByOffset[prefixBlock.Instructions.First().Offset] = prefixBlock;
                blocksByOffset[toInstr.Offset] = containingBlock;

                prefixBlock.OutboundEdges.Add(containingBlock);
            }

            foreach (var (from, toInstr) in edges)
            {
                if (!blocksByOffset.TryGetValue(toInstr.Offset, out var to))
                    throw new InvalidOperationException("Blocks were not split correctly");
                from.OutboundEdges.Add(to);
            }
        }

        /// <remarks>Using Depth-First Search</remarks>
        private void SetPostOrderNumber()
        {
            var visited = new HashSet<Block>(allBlocks.Count);
            var stack = new Stack<(Block, int)>();
            stack.Push((allBlocks[0], 0));
            int next = 0;
            while (stack.Any())
            {
                var (parent, edgeI) = stack.Pop();
                visited.Add(parent);
                if (edgeI < parent.OutboundEdges.Count)
                {
                    var child = parent.OutboundEdges[edgeI];
                    stack.Push((parent, ++edgeI));
                    if (!visited.Contains(child))
                        stack.Push((child, 0));
                }
                else
                    parent.PostOrderI = next++;
            }
        }

        /// <remarks>The postorder of the reversed graph</remarks>
        private void SetPostOrderRevNumber()
        {
            var visited = new HashSet<Block>(allBlocks.Count);
            var stack = new Stack<(Block, int)>();
            foreach (var startBlock in allBlocks.Where(b => b.OutboundEdges.Count == 0))
                stack.Push((startBlock, 0));
            int next = 0;
            while (stack.Any())
            {
                var (parent, edgeI) = stack.Pop();
                visited.Add(parent);
                if (edgeI < parent.InboundForwardEdges.Count + parent.InboundBackwardEdges.Count)
                {
                    var child = edgeI < parent.InboundForwardEdges.Count
                        ? parent.InboundForwardEdges[edgeI]
                        : parent.InboundBackwardEdges[edgeI - parent.InboundForwardEdges.Count];
                    stack.Push((parent, ++edgeI));
                    if (!visited.Contains(child))
                        stack.Push((child, 0));
                }
                else
                    parent.PostOrderRevI = next++;
            }
        }

        /// <remarks>Again using Depth-First Search, but pre-order this time</remarks>
        private void SetInboundEdges()
        {
            var visited = new HashSet<Block>(allBlocks.Count);
            var stack = new Stack<(Block, int)>();
            stack.Push((allBlocks[0], 0));
            while (stack.Any())
            {
                var (parent, edgeI) = stack.Pop();
                visited.Add(parent);
                if (edgeI >= parent.OutboundEdges.Count)
                    continue;
                var child = parent.OutboundEdges[edgeI];

                (stack.Any(t => t.Item1 == child)
                    ? child.InboundBackwardEdges
                    : child.InboundForwardEdges).Add(parent);

                stack.Push((parent, edgeI + 1));
                if (!visited.Contains(child))
                    stack.Push((child, 0));
            }
        }

        private void ConstructLoops()
        {
            var continueBlocks = allBlocks.Where(b => b.InboundBackwardEdges.Any());
            if (continueBlocks.Any())
                throw new NotSupportedException("Loops are currently not supported");
            foreach (var continueBlock in continueBlocks)
            {
                var backEdgeBlock = continueBlock.InboundBackwardEdges.Count > 1
                    ? ConstructBackEdgeBlock(continueBlock.InboundBackwardEdges)
                    : continueBlock.InboundBackwardEdges.Single();

            }
        }

        private IControlFlowBlock ConstructBackEdgeBlock(IReadOnlyList<IControlFlowBlock> sourceBlocks)
        {
            throw new NotSupportedException("Unsupported loop construct with multiple back edges");
        }

        /// <summary>After constructing loops we should have a DAG and all branches are selections</summary>
        private void ConstructSelections()
        {
            var headers = allBlocks.Where(b => b.OutboundEdges.Count > 1);
            foreach (var header in headers)
            {
                var mergeBlock = header.OutboundEdges
                    .Select(s => s.PostDominators)
                    .Aggregate((a, b) => a.Intersect(b).ToArray())
                    .FirstOrDefault();
                if (mergeBlock == null) // TODO: maybe just add an unreachable merge block?
                    throw new NotSupportedException("Unsupported undivergent selection control flow");

                header.BlockKinds |= BlockKinds.SelectionHeader;
                header.MergeBlock = mergeBlock;
            }
        }
    }
}
