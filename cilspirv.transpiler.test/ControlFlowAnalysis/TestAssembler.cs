using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace cilspirv.transpiler.test.ControlFlowAnalysis
{
    public class TestAssembler
    {
        [Test]
        public void SimplestMethod()
        {
            var method = Assembler.Parse("ret // ignore this");
            Assert.AreEqual("Func", method.Name);
            Assert.AreEqual(1, method.Body.Instructions.Count);
            Assert.AreEqual(OpCodes.Ret, method.Body.Instructions.Single().OpCode);
        }
    }
}
