using System;
using System.Numerics;
using cilspirv.Library;
using cilspirv.Spirv;

namespace test_shaders
{
    [Capability(Capability.Shader)]
    [MemoryModel(AddressingModel.Logical, MemoryModel.GLSL450)]
    public class ExplicitUniform
    {
        public struct VSInput
        {
            [Location(0)] public Vector3 pos;
            [Location(1)] public Vector2 uv;
            [Location(2)] public Vector4 color;
        }

        public struct FSInput
        {
            [Location(0)] public Vector2 uv;
            [Location(1)] public Vector4 color;
        }

        [Block]
        public struct Uniforms
        {
            public const int Size = sizeof(float) * (4 + 4 * 4);

            public Vector4 actualColor;
            public Matrix4x4 someMatrix;
        }

        [EntryPoint(ExecutionModel.Fragment)]
        public void Frag(
            [Input] in FSInput input,

            [Uniform, Binding(0, 0)] Uniforms uniforms,

            [Output, Location(0)] out Vector4 output)
        {
            output = uniforms.actualColor;
        }

        [EntryPoint(ExecutionModel.Vertex)]
        public void Vert(
            [Input] in VSInput input,

            [Output, BuiltIn(BuiltIn.Position)] out Vector4 position,
            [Output] out FSInput output)
        {
            position = new Vector4(input.pos, 1f);
            output.uv = input.uv;
            output.color = input.color;
        }
    }
}
