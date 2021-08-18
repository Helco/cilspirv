using System;
using System.Numerics;
using cilspirv.Library;
using cilspirv.Spirv;

namespace test_shaders
{
#pragma warning disable CS0649
    [Capability(Capability.Shader)]
    [MemoryModel(AddressingModel.Logical, MemoryModel.GLSL450)]
    public class Simple
    {
        [Input]
        public struct VSInput
        {
            [Location(0)] public Vector3 pos;
            [Location(1)] public Vector4 color;
        }

        [EntryPoint(ExecutionModel.Fragment)]
        public void Frag(
            [Input, Location(0)] in Vector4 color,
            [Output, Location(0)] out Vector4 output)
        {
            output = color;
        }

        [EntryPoint(ExecutionModel.Vertex)]
        public void Vert(
            in VSInput input,

            [Uniform, Binding(0, 0)] in Matrix4x4 projection,
            [Uniform, Binding(0, 0)] in Matrix4x4 view,
            [Uniform, Binding(0, 0)] in Matrix4x4 world,

            [Output, BuiltIn(BuiltIn.Position)] out Vector4 position,
            [Output, Location(0)] out Vector4 fsout_color)
        {
            position = new Vector4(input.pos, 1f);
            position = Vector4.Transform(position, world);
            position = Vector4.Transform(position, view);
            position = Vector4.Transform(position, projection);
            fsout_color = input.color;
        }
    }
}
