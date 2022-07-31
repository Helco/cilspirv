using System;
using NUnit.Framework;
using cilspirv.Library;
using cilspirv.Spirv;
using System.Linq;
using System.IO;

namespace cilspirv.transpiler.test.Annotations.Modules
{
    public class Decorations
    {
        public struct Uniforms
        {
            [Offset(0)] public int a;
        }

        [Block]
        public struct UniformsBlock
        {
            [Offset(0)] public int a;
        }

        [EntryPoint(ExecutionModel.Vertex)]
        public void Struct([Uniform, Binding(0, 5)] UniformsBlock u) { }

        [EntryPoint(ExecutionModel.Vertex)]
        public void ImplicitUniformBlockStruct([Uniform, Binding(0, 5)] Uniforms u) { }
    }
}

namespace cilspirv.transpiler.test.Annotations
{
    public class Dcorations : ApprovalTranspileFixture<Modules.Decorations>
    {
        [Test] public void Struct() => VerifyEntryPoint(
            nameof(Modules.Decorations.Struct),
            new Transpiler.TranspilerOptions()
            {
                ImplicitUniformBlockStructures = false
            });

        [Test] public void ImplicitUniformBlockStruct() => VerifyEntryPoint(
            nameof(Modules.Decorations.ImplicitUniformBlockStruct),
            new Transpiler.TranspilerOptions()
            {
                ImplicitUniformBlockStructures = true
            });
    }
}
