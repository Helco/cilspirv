using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace cilspirv.transpiler.test
{
    public class TestAssembler
    {
#pragma warning disable CS0169
        struct Struct { }
        void Method() { }
        int Field;
#pragma warning restore CS0169

        [Test]
        public void SimplestMethod()
        {
            var method = Assembler.Parse("ret // ignore this");
            Assert.AreEqual("Func", method.Name);
            Assert.AreEqual(1, method.Body.Instructions.Count);
            Assert.AreEqual(OpCodes.Ret, method.Body.Instructions.Single().OpCode);
        }

        [Test]
        public void MultipleInstructions()
        {
            var method = Assembler.Parse(@"
    ldarg.0
    stloc.3
L0: ret
");

            Assert.AreEqual(new[]
            {
                OpCodes.Ldarg_0,
                OpCodes.Stloc_3,
                OpCodes.Ret
            }, method.Body.Instructions.Select(i => i.OpCode).ToArray());
        }

        [Test]
        public void TypeReference()
        {
            var method = Assembler.Parse(@"stobj cilspirv.transpiler.test.TestAssembler+Struct");

            var instr = method.Body.Instructions.Single();
            Assert.AreEqual(OpCodes.Stobj, instr.OpCode);
            Assert.IsInstanceOf<TypeReference>(instr.Operand);
            Assert.AreEqual("cilspirv.transpiler.test.TestAssembler/Struct", ((TypeReference)instr.Operand).FullName);
            // Mono uses / as separator between parent and child type, CLR uses +
        }

        [Test]
        public void MethodReference()
        {
            var method = Assembler.Parse(@"call cilspirv.transpiler.test.TestAssembler::Method");

            var instr = method.Body.Instructions.Single();
            Assert.AreEqual(OpCodes.Call, instr.OpCode);
            Assert.IsInstanceOf<MethodReference>(instr.Operand);
            Assert.AreEqual("System.Void cilspirv.transpiler.test.TestAssembler::Method()", ((MethodReference)instr.Operand).FullName);
        }

        [Test]
        public void FieldReference()
        {
            var method = Assembler.Parse(@"ldfld cilspirv.transpiler.test.TestAssembler::Field");

            var instr = method.Body.Instructions.Single();
            Assert.AreEqual(OpCodes.Ldfld, instr.OpCode);
            Assert.IsInstanceOf<FieldReference>(instr.Operand);
            Assert.AreEqual("System.Int32 cilspirv.transpiler.test.TestAssembler::Field", ((FieldReference)instr.Operand).FullName);
        }

        [Test]
        public void Literals()
        {
            var method = Assembler.Parse(@"
ldc.i4.s 4
ldc.i4 400
ldc.i8 1000000000000
ldc.r4 3.1415
ldc.r8 3.141592653589
ldstr abcdef
ldstr ""defghi""
");

            var operands = method.Body.Instructions.Select(i => i.Operand).ToArray();
            Assert.AreEqual((byte)4, operands[0]);
            Assert.AreEqual(400, operands[1]);
            Assert.AreEqual(1000000000000L, operands[2]);
            Assert.AreEqual(3.1415f, operands[3]);
            Assert.AreEqual(3.141592653589, operands[4]);
            Assert.AreEqual("abcdef", operands[5]);
            Assert.AreEqual("defghi", operands[6]);
        }

        [Test]
        public void Parameters()
        {
            var method = Assembler.Parse(@"
.param System.Int32
.param System.String
.return System.Boolean
ldarg 0
ldarg.s 1
");
            var operands = method.Body.Instructions.Select(i => i.Operand).ToArray();
            Assert.AreSame(method.Parameters[0], operands[0]);
            Assert.AreSame(method.Parameters[1], operands[1]);
        }

        [Test]
        public void Variables()
        {
            var method = Assembler.Parse(@"
.local System.Int32
.local System.String
.local System.Boolean
ldloc 0
ldloc.s 1
");
            var operands = method.Body.Instructions.Select(i => i.Operand).ToArray();
            Assert.AreSame(method.Body.Variables[0], operands[0]);
            Assert.AreSame(method.Body.Variables[1], operands[1]);
        }

        [Test]
        public void BranchTargets()
        {
            var method = Assembler.Parse(@"
L0: br L1
L1: br L0
L2: switch L0, L1, L2
");

            var instrs = method.Body.Instructions.ToArray();
            Assert.AreSame(instrs[1], instrs[0].Operand);
            Assert.AreSame(instrs[0], instrs[1].Operand);
            Assert.AreEqual(instrs, instrs[2].Operand);
        }
    }
}
