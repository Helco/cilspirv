using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cilspirv.SourceGen;

partial class SourceGen
{
    public const int DimCube = -1;

    private readonly struct ImageOptions
    {
        public readonly bool Array;
        public readonly bool Depth;
        public readonly bool MultiSampled;

        public ImageOptions(bool array, bool depth, bool multiSampled)
        {
            Array = array;
            Depth = depth;
            MultiSampled = multiSampled;
        }
    }

    private static readonly int[] ImageDims = new[] { 1, 2, 3, DimCube };

    private static readonly ImageOptions[] ImageOptionSets = new ImageOptions[]
    {
        new(array: false, depth: false, multiSampled: false),
        new(array: true, depth: false, multiSampled: false),
        new(array: false, depth: true, multiSampled: false),
        new(array: true, depth: true, multiSampled: false)
    };

    private static void GenerateImageClasses(string outputPath)
    {
        using var writer = new StreamWriter(outputPath);

        writer.WriteLine("// This file was generated. Do not modify.");
        writer.WriteLine("using System;");
        writer.WriteLine("using System.Numerics;");
        writer.WriteLine("using cilspirv.Spirv;");
        writer.WriteLine();
        writer.WriteLine("namespace cilspirv.Library;");

        foreach (var dim in ImageDims)
        {
            foreach (var options in ImageOptionSets)
            {
                writer.WriteLine();
                writer.WriteLines(ImageType(dim, options));
                writer.WriteLine();
                writer.WriteLines(SamplerType(dim, options));
            }
        }

        static string DimToEnum(int dim) =>
            dim == DimCube ? "Dim.Cube" : $"Dim.Dim{dim}D";

        static string DimToName(int dim) =>
            dim == DimCube ? "Cube" : dim + "D";

        static string BoolToString(bool b) =>
            b.ToString().ToLowerInvariant();

        static string ImageInfoAttribute(int dim, ImageOptions options) =>
            $"[ImageInfo(Dim = {DimToEnum(dim)}, " +
            $"Array = {BoolToString(options.Array)}, " +
            $"Depth = {BoolToString(options.Depth)}, "+
            $"MultiSampled = {BoolToString(options.MultiSampled)})]";

        static string Name(string baseName, int dim, ImageOptions options) =>
            $"{baseName}{DimToName(dim)}" +
            (options.MultiSampled ? "MS" : "") +
            (options.Depth ? "Depth" : "") +
            (options.Array ? "Array" : "");

        static string ImageName(int dim, ImageOptions options) => Name("Image", dim, options);
        static string SamplerName(int dim, ImageOptions options) => Name("Sampler", dim, options);

        static IEnumerable<string> ImageType(int dim, ImageOptions options)
        {
            yield return ImageInfoAttribute(dim, options);
            yield return $"public interface {ImageName(dim, options)}";
            yield return "{";
            yield return $"    {SamplerName(dim, options)} Sampled(Sampler sampler);";
            yield return "}";
        }

        static IEnumerable<string> SamplerInterfaces(int dim, ImageOptions options)
        {
            if (options.MultiSampled == false)
            {
                yield return "ISamplable";
                yield return "ILODSamplable";
            }
            if (dim is 2 or DimCube && options.MultiSampled == false)
                yield return "IGatherable";
        }

        static string CoordinateType(int dim, ImageOptions options)
        {
            var componentCount = (dim == DimCube ? 3 : dim) + (options.Array ? 1 : 0);
            return componentCount > 1 ? "Vector" + componentCount : "float";
        }

        static IEnumerable<string> SamplerType(int dim, ImageOptions options)
        {
            var interfaces = SamplerInterfaces(dim, options).ToArray();
            var coordinateType = CoordinateType(dim, options);

            yield return ImageInfoAttribute(dim, options);
            yield return $"public interface {SamplerName(dim, options)} :";
            foreach (var i in interfaces.SkipLast(1))
                yield return $"    {i}<Vector4, {coordinateType}>,";
            yield return $"    {interfaces.Last()}<Vector4, {coordinateType}>";
            yield return "{";
            yield return $"    {ImageName(dim, options)} Image {{ get; }}";
            yield return "}";
        }
    }
}
