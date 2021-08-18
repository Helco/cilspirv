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
        private struct Color
        {
            public float r, g, b, a;
        }

        [Input]
        private struct VSInput
        {
            [Location(0)] public Vector4 pos;
            [Location(1)] public Color color;
        }
        
        private VSInput input;
        [Output, BuiltIn(BuiltIn.Position)]
        private Vector4 position;

        public Vector4 DoNothing()
        {
            return new Vector4(0.1f, 0.3f, 0.7f, 1f);
        }

        [EntryPoint(ExecutionModel.Fragment)]
        public void Frag([Output, Location(0)] Vector4 output)
        {
            output = DoNothing();
        }

        [EntryPoint(ExecutionModel.Vertex)]
        public void Vert()
        {
            position = new Vector4(input.color.r, input.color.g, input.color.b, input.color.a);
        }
    }
}
