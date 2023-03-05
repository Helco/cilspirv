using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using cilspirv.Spirv;
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

    private static IReadOnlyDictionary<string, Type> TypeByName(Type[] types) =>
        types.ToDictionary(t => t.FullName!, t => t);

    public IMappedFromCILType? TryMapType(TypeReference ilTypeRef)
    {
        if (ImageTypesByName.TryGetValue(ilTypeRef.FullName, out var imageType))
            return MapImageType(imageType);
        if (SamplerTypesByName.TryGetValue(ilTypeRef.FullName, out var samplerType))
            return MapSamplerType(samplerType);
        return null;
    }

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
            Access = AccessQualifier.ReadOnly,
            HasSampler = true,
            UserName = name
        };
    }

    private static SpirvType MapSamplerType(Type type)
    {
        var imageInfo = GetImageInfo(type);
        return new SpirvSampledImageType()
        {
            ImageType = MapImageType(imageInfo, type.Name + "_Image"),
            UserName = type.Name
        };
    }
}
