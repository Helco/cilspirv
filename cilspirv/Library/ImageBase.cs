using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cilspirv.Spirv;

namespace cilspirv.Library;

public class ImageInfoAttribute : Attribute
{
    public Dim Dim { get; init; }
    public bool Array { get; init; }
    public bool Depth { get; init; }
    public bool MultiSampled { get; init; }
}

public interface Sampler { }

public interface ISamplable<TResult, TFCoordinate>
    where TResult : struct
    where TFCoordinate : struct
{
    TResult Sample(TFCoordinate coord);
}

public interface ILODSamplable<TResult, TFCoordinate>
    : ISamplable<TResult, TFCoordinate>
    where TResult : struct
    where TFCoordinate : struct
{
    TResult SampleLOD(TFCoordinate coord, int lod);
    TResult SampleLOD(TFCoordinate coord, uint lod);
}

public interface IGatherable<TResult, TFCoordinate>
{
    TResult Gather(TFCoordinate coord, int component);
    TResult GatherR(TFCoordinate coord) => Gather(coord, 0);
    TResult GatherG(TFCoordinate coord) => Gather(coord, 1);
    TResult GatherB(TFCoordinate coord) => Gather(coord, 2);
    TResult GatherA(TFCoordinate coord) => Gather(coord, 3);
}

