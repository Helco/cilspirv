using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler;
using Mono.Cecil;

namespace cilspirv.Library;

internal class ProvidedLibraryMapper : ITranspilerLibraryMapper
{
    private static readonly Type[] ImageTypes = new[]
    {
        typeof(Image1D),
        typeof(Image1DArray),
        typeof(Image1DDepth),
        typeof(Image1DDepthArray),
        typeof(Image2D),
        typeof(Image2DArray),
        typeof(Image2DDepth),
        typeof(Image2DDepthArray),
        typeof(Image3D),
        typeof(Image3DArray),
        typeof(Image3DDepth),
        typeof(Image3DDepthArray),
        typeof(ImageCube),
        typeof(ImageCubeArray),
        typeof(ImageCubeDepth),
        typeof(ImageCubeDepthArray),
    };
    private static readonly Type[] SamplerTypes = new[]
    {
        typeof(Sampler1D),
        typeof(Sampler1DArray),
        typeof(Sampler1DDepth),
        typeof(Sampler1DDepthArray),
        typeof(Sampler2D),
        typeof(Sampler2DArray),
        typeof(Sampler2DDepth),
        typeof(Sampler2DDepthArray),
        typeof(Sampler3D),
        typeof(Sampler3DArray),
        typeof(Sampler3DDepth),
        typeof(Sampler3DDepthArray),
        typeof(SamplerCube),
        typeof(SamplerCubeArray),
        typeof(SamplerCubeDepth),
        typeof(SamplerCubeDepthArray),
    };
    private static readonly IReadOnlyDictionary<string, Type> ImageTypesByName = TypeByName(ImageTypes);
    private static readonly IReadOnlyDictionary<string, Type> SamplerTypesByName = TypeByName(SamplerTypes);

    private static readonly Type[] SampleableTypes = new[]
    {
        typeof(ISamplable<Vector4, float>),
        typeof(ISamplable<Vector4, Vector2>),
        typeof(ISamplable<Vector4, Vector3>),
        typeof(ISamplable<Vector4, Vector4>),
    };

    private static readonly Type[] CombineableTypes = new[]
    {
        typeof(Image1D),
        typeof(Image1DArray),
        typeof(Image1DDepth),
        typeof(Image1DDepthArray),
        typeof(Image2D),
        typeof(Image2DArray),
        typeof(Image2DDepth),
        typeof(Image2DDepthArray),
        typeof(Image3D),
        typeof(Image3DArray),
        typeof(Image3DDepth),
        typeof(Image3DDepthArray),
        typeof(ImageCube),
        typeof(ImageCubeArray),
        typeof(ImageCubeDepth),
        typeof(ImageCubeDepthArray)
    };

    private static IReadOnlyDictionary<string, Type> TypeByName(Type[] types) =>
        types.ToDictionary(t => t.FullName!, t => t);

    private readonly ExternalMethodMapper methodMapper = new();

    public ProvidedLibraryMapper()
    {
        foreach (var sampleableType in SampleableTypes)
            methodMapper.Add(ExternalMethodMapper.FullNameOf(sampleableType, "Sample"), GenerateSample);
        foreach (var combineableType in CombineableTypes)
            methodMapper.Add(ExternalMethodMapper.FullNameOf(combineableType, "Sampled"), GenerateCombine);
    }

    public IMappedFromCILType? TryMapType(TypeReference ilTypeRef)
    {
        if (ImageTypesByName.TryGetValue(ilTypeRef.FullName, out var imageType))
            return MapImageType(imageType);
        if (SamplerTypesByName.TryGetValue(ilTypeRef.FullName, out var samplerType))
            return MapSampledImageType(samplerType);
        if (ilTypeRef.FullName == typeof(Sampler).FullName)
            return MapSamplerType(ilTypeRef);
        return null;
    }

    public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => methodMapper.TryMapMethod(methodRef);

    private static ImageInfoAttribute GetImageInfo(Type type) =>
        type.GetCustomAttribute<ImageInfoAttribute>() ?? 
        throw new ArgumentException("Given type has no ImageInfoAttribute");

    private static SpirvType MapImageType(Type type) =>
        MapImageType(GetImageInfo(type), type.Name);

    private static SpirvImageType MapImageType(ImageInfoAttribute imageInfo, string name)
    {
        return new SpirvImageType()
        {
            Dim = imageInfo.Dim,
            IsArray = imageInfo.Array,
            IsDepth = imageInfo.Depth,
            IsMultisampled = imageInfo.MultiSampled,
            SampledType = new SpirvFloatingType() { Width = 32 },
            Format = ImageFormat.Unknown,
            HasSampler = true,
            UserName = name
        };
    }

    private static SpirvType MapSampledImageType(Type type)
    {
        var imageInfo = GetImageInfo(type);
        return new SpirvSampledImageType()
        {
            ImageType = MapImageType(imageInfo, type.Name + "_Image"),
            UserName = type.Name
        };
    }

    private static SpirvType MapSamplerType(TypeReference type)
    {
        return new SpirvSamplerType()
        {
            UserName = type.Name
        };
    }

    private static IEnumerable<Instruction> GenerateSample(ITranspilerMethodContext ctx)
    {
        var thiz = ctx.This ?? throw new InvalidOperationException("Encountered sample method without this parameter");
        var thizType = (SpirvSampledImageType)thiz.type;
        var resultType = new SpirvVectorType()
        {
            ComponentType = thizType.ImageType!.SampledType!,
            ComponentCount = 4
        };
        yield return new OpImageSampleImplicitLod()
        {
            Result = ctx.ResultID = ctx.CreateID(),
            ResultType = ctx.IDOf(resultType),
            SampledImage = thiz.id,
            Coordinate = ctx.Parameters[0].id
        };
    }

    private static IEnumerable<Instruction> GenerateCombine(ITranspilerMethodContext ctx)
    {
        var thiz = ctx.This ?? throw new InvalidOperationException("Encountered combine method without this parameter");
        var imageType = (SpirvImageType)thiz.type;
        var resultType = new SpirvSampledImageType()
        {
            ImageType = imageType,
            UserName = imageType.UserName + "_Combined"
        };
        yield return new OpSampledImage()
        {
            Result = ctx.ResultID = ctx.CreateID(),
            ResultType = ctx.IDOf(resultType),
            Image = thiz.id,
            Sampler = ctx.Parameters[0].id
        };
    }
}
