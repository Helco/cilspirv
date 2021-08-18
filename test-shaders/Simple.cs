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
        [Uniform, Binding(0, 0)]
        public struct UniformBlock
        {
            public Vector4 color;
        }

        [EntryPoint(ExecutionModel.Fragment)]
        public void Frag(
            in UniformBlock uniform,
            [Output, Location(0)] out Vector4 output)
        {
            output = uniform.color;
        }

        [EntryPoint(ExecutionModel.Vertex)]
        public void Vert(
            [Input, Location(0)] in Vector3 input,
            [Output, BuiltIn(BuiltIn.Position)] out Vector4 position)
        {
            position = new Vector4(input, 1f);
        }
    }
}
