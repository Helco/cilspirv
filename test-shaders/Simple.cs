using System;
using System.Numerics;
using cilspirv.Library;
using cilspirv.Spirv;

namespace test_shaders
{
    [Capability(Capability.Shader)]
    [MemoryModel(AddressingModel.Logical, MemoryModel.GLSL450)]
    public class Simple
    {
        [Output, Location(0)]
        private Vector4 output;
        [Input, Location(0)]
        private Vector4 input = default;
        [Output, BuiltIn(BuiltIn.Position)]
        private Vector4 position;

        [EntryPoint(ExecutionModel.Fragment)]
        public void Frag()
        {
            output = new Vector4(0.1f, 0.3f, 0.7f, 1f);
        }

        [EntryPoint(ExecutionModel.Vertex)]
        public void Vert()
        {
            position = input;
        }
    }
}
