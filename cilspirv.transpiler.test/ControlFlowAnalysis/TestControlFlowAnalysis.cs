﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;

namespace cilspirv.Transpiler.test
{
    [UseReporter(typeof(DiffReporter))]
    public class TestControlFlowAnalysis
    {
        internal void ApproveCFABlocks(ControlFlowAnalysis cfa)
        {
            var writer = new StringWriter();
            writer.NewLine = "\n";
            int i = 0;
            foreach (var block in cfa.Blocks.Cast<ControlFlowAnalysis.Block>())
            {
                if (i != 0)
                    writer.WriteLine();
                writer.Write("[{0}] {1}", i, block.BlockKinds);
                if (block.HeaderBlockKind != HeaderBlockKind.None)
                {
                    writer.Write(" Merge:");
                    WriteBlock(block.MergeBlock);
                }
                if (block.HeaderBlockKind == HeaderBlockKind.Loop)
                {
                    writer.Write(" Continue:");
                    WriteBlock(block.ContinueBlock);
                }
                writer.WriteLine();

                writer.Write("  Edges: Out");
                foreach (var b in block.OutboundEdges)
                {
                    writer.Write('-');
                    WriteBlock(b);
                }
                writer.Write("  In");
                foreach (var b in block.InboundForwardEdges)
                {
                    writer.Write('-');
                    WriteBlock(b);
                }
                writer.Write("  Back");
                foreach (var b in block.InboundBackwardEdges)
                {
                    writer.Write('-');
                    WriteBlock(b);
                }
                writer.WriteLine();

                //writer.Write("  Order: Post-");
                //writer.Write(block.PostOrderI);
                //writer.Write("  PostRev-");
                //writer.WriteLine(block.PostOrderRevI);

                writer.Write("  Dominators: Pre-");
                WriteBlock(block.ImmediateDominator);
                writer.Write("  Post-");
                WriteBlock(block.ImmediatePostDominator);
                writer.WriteLine();

                foreach (var instr in block.Instructions)
                {
                    writer.Write("    ");
                    writer.WriteLine(instr);
                }

                i++;
            }
            void WriteBlock(ControlFlowAnalysis.Block? block)
            {
                writer!.Write(block == null ? "<none>" : cfa.Blocks.IndexOf(block));
            }
                

            Approvals.Verify(writer.ToString(), scrubber: input => input.Replace("\r\n", "\n"));
        }

        internal void ApproveCFABlocks(string cil, bool ignoreLoopExceptions = false)
        {
            var method = Assembler.Parse(cil);
            var cfa = new ControlFlowAnalysis(method.Body);
            try
            {
                cfa.Analyse();
            }
            catch (NotSupportedException e) when (ignoreLoopExceptions && e.Message.Contains("Loops are currently not supported"))
            { }
            ApproveCFABlocks(cfa);
        }

        [Test]
        public void SingleBlock() => ApproveCFABlocks(@"
    ret
");

        [Test]
        public void UnconditionalBranches() => ApproveCFABlocks(@"
    br L1
L1: br L2
L2: br L3
L3: br L4
L4: ret
");

        [Test]
        public void SingleCondition() => ApproveCFABlocks(@"
.param System.Boolean
    ldarg.0
    brfalse L0
    ldarg.1
    br L1
L0: ldarg.2
L1: ret");

        [Test]
        public void SimpleLoop() => ApproveCFABlocks(@"
.param System.Int32
L0: ldarg.0
    brfalse LX
    ldarg.0
    ldc.i4.1
    sub
    starg 0
    br L0
LX: ret",
            ignoreLoopExceptions: true);

        [Test]
        public void SimpleLoopLaterCond() => ApproveCFABlocks(@"
.param System.Int32
    br LC
L0: ldarg.0
    ldc.i4.1
    sub
    starg 0
LC: ldarg.0
    brtrue L0
    ret",
            ignoreLoopExceptions: true);

    }
}
