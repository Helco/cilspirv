using System;
using NUnit.Framework;
using cilspirv.Library;
using cilspirv.Spirv;
using System.Linq;
using System.IO;

namespace cilspirv.transpiler.test.Values.Modules
{
    public class TemplateStructs
    {
        public struct Simple
        {
            [Location(0)] public float m;
        }

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public float RefFrag([Input] in Simple simple) => simple.m * 2;

        [EntryPoint(ExecutionModel.Vertex)]
        public void RefVert([Output] out Simple simple) => simple.m = 42;

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public float ValFrag([Input] Simple simple) => simple.m * 2;

        [EntryPoint(ExecutionModel.Vertex)]
        public void ValVert([Output] Simple simple) => simple.m = 42;

        [EntryPoint(ExecutionModel.Vertex)]
        [return: Output, Location(0)]
        public Simple ValReturnVert() => new() { m = 42 };
    }
}

namespace cilspirv.transpiler.test.Values
{
    public class TemplateStructs : ApprovalTranspileFixture<Modules.TemplateStructs>
    {
        [Test]
        public void RefShared() => VerifyModule(
            (nameof(Modules.TemplateStructs.RefFrag), isEntryPoint: true),
            (nameof(Modules.TemplateStructs.RefVert), isEntryPoint: true));

        [Test]
        public void ValShared() => VerifyModule(
            (nameof(Modules.TemplateStructs.ValFrag), isEntryPoint: true),
            (nameof(Modules.TemplateStructs.ValVert), isEntryPoint: true));

        /*[Test] initobj is not implemented, local vars can only be SPIRV
        public void ValReturnShared() => VerifyModule(
            (nameof(Modules.TemplateStructs.ValFrag), isEntryPoint: true),
            (nameof(Modules.TemplateStructs.ValReturnVert), isEntryPoint: true));*/
    }
}
