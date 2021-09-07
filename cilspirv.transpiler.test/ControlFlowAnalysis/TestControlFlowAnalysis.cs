using System;
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
            foreach (var block in cfa.Blocks)
            {
                if (i != 0)
                    writer.WriteLine();
                writer.Write("[{0}] {1}", i, block.HeaderBlockKind);
                if (block.HeaderBlockKind != HeaderBlockKind.None)
                {
                    writer.Write(" Merge:");
                    writer.Write(cfa.Blocks.IndexOf(block.MergeBlock));
                }
                if (block.HeaderBlockKind == HeaderBlockKind.Loop)
                {
                    writer.Write(" Continue:");
                    writer.Write(cfa.Blocks.IndexOf(block.ContinueBlock));
                }
                writer.WriteLine();
                foreach (var instr in block.Instructions)
                {
                    writer.Write("    ");
                    writer.WriteLine(instr);
                }

                i++;
            }

            Approvals.Verify(writer.ToString(), scrubber: input => input.Replace("\r\n", "\n"));
        }

        internal void ApproveCFABlocks(string cil)
        {
            var method = Assembler.Parse(cil);
            var cfa = new ControlFlowAnalysis(method.Body);
            cfa.Analyse();
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
L2: br L4
L3: nop
L4: br L5
L5: ret
");

        [Test]
        public void SingleCondition() => ApproveCFABlocks(@"
    ldarg.0
    brfalse L0
    ldarg.1
    br L1
L0: ldarg.2
L1: ret");
    }
}
