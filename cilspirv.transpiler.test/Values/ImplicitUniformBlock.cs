using System;
using NUnit.Framework;
using cilspirv.Library;
using cilspirv.Spirv;
using System.Linq;
using System.IO;

namespace cilspirv.transpiler.test.Values.Modules
{
    public class ImplicitUniformBlock
    {
        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int Primitive([Uniform, Binding(0, 0)] int a) => a;

        public struct PrimitiveStruct
        {
            [Offset(0)] public int a;
            [Offset(4)] public int b;
            [Offset(8)] public int c;
        }

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int PrimStructFirst([Uniform, Binding(0, 0)] PrimitiveStruct s) => s.a;

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int PrimStructLast([Uniform, Binding(0, 0)] PrimitiveStruct s) => s.c;

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int RefPrimStructFirst([Uniform, Binding(0, 0)] in PrimitiveStruct s) => s.a;

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int RefPrimStructLast([Uniform, Binding(0, 0)] in PrimitiveStruct s) => s.c;

        public struct NestedStruct
        {
            [Offset(0)] public int a;
            [Offset(16)] public PrimitiveStruct nested;
        }

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int ValueNestedStruct([Uniform, Binding(0, 0)] NestedStruct n) => n.nested.b;

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int RefNestedStruct([Uniform, Binding(0, 0)] in NestedStruct n) => n.nested.b;
    }
}

namespace cilspirv.transpiler.test.Values
{
    public class TestImplicitUniformBlock : ApprovalTranspileFixture<Modules.ImplicitUniformBlock>
    {
        [Test] public void Primitive() => VerifyEntryPoint(nameof(Modules.ImplicitUniformBlock.Primitive));
        [Test] public void PrimStructFirst() => VerifyEntryPoint(nameof(Modules.ImplicitUniformBlock.PrimStructFirst));
        [Test] public void PrimStructLast() => VerifyEntryPoint(nameof(Modules.ImplicitUniformBlock.PrimStructLast));
        [Test] public void RefPrimStructFirst() => VerifyEntryPoint(nameof(Modules.ImplicitUniformBlock.RefPrimStructFirst));
        [Test] public void RefPrimStructLast() => VerifyEntryPoint(nameof(Modules.ImplicitUniformBlock.RefPrimStructLast));
        [Test] public void ValueNestedStruct() => VerifyEntryPoint(nameof(Modules.ImplicitUniformBlock.ValueNestedStruct));
        [Test] public void RefNestedStruct() => VerifyEntryPoint(nameof(Modules.ImplicitUniformBlock.RefNestedStruct));
    }
}
