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
        [EntryPoint(ExecutionModel.Fragment)]
        public Vector4 Frag()
        {
            return new Vector4(0.1f, 0.3f, 0.7f, 1f);
        }
    }
}
