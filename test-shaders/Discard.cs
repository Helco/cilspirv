using System;
using System.Numerics;
using cilspirv.Library;
using cilspirv.Spirv;

namespace test_shaders
{
    [Capability(Capability.Shader)]
    [MemoryModel(AddressingModel.Logical, MemoryModel.GLSL450)]
    public class Discard
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

        [EntryPoint(ExecutionModel.Fragment)]
        public void Frag(
            [Input] in FSInput input,

            [Output, Location(0)] out Vector4 output)
        {
            output = input.color;
            int checker = (int)(input.uv.X * 8f) + (int)(input.uv.Y * 8);
            if ((checker % 2) == 0)
                Environment.Exit(0);
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
