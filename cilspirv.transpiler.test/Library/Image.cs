using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using cilspirv.Library;
using cilspirv.Spirv;
using NUnit.Framework;

namespace cilspirv.transpiler.test.Library.Modules
{
    public class Image
    {
        [EntryPoint(Spirv.ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public Vector4 SampleConstant([UniformConstant, Binding(0, 0)] Sampler2D sampler) =>
            sampler.Sample(new(0f));

        [EntryPoint(Spirv.ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public Vector4 AssembleSampler(
            [UniformConstant, Binding(0, 0)] Image2D image,
            [UniformConstant, Binding(0, 1)] Sampler sampler) =>
            image.Sampled(sampler).Sample(new(0f));
    }
}

namespace cilspirv.transpiler.test.Library
{
    public class TestImage : ApprovalTranspileFixture<Modules.Image>
    {
        [Test] public void SampleConstant() => VerifyEntryPoint(nameof(Modules.Image.SampleConstant));
        [Test] public void AssembleSampler() => VerifyEntryPoint(nameof(Modules.Image.AssembleSampler));

    }
}
