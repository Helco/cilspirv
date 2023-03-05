// This file was generated. Do not modify.
using System;
using System.Numerics;
using cilspirv.Spirv;

namespace cilspirv.Library;

[ImageInfo(Dim = Dim.Dim1D, Array = false, Depth = false, MultiSampled = false)]
public interface Image1D
{
    Sampler1D Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim1D, Array = false, Depth = false, MultiSampled = false)]
public interface Sampler1D :
    ISamplable<Vector4, float>,
    ILODSamplable<Vector4, float>
{
    Image1D Image { get; }
}

[ImageInfo(Dim = Dim.Dim1D, Array = true, Depth = false, MultiSampled = false)]
public interface Image1DArray
{
    Sampler1DArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim1D, Array = true, Depth = false, MultiSampled = false)]
public interface Sampler1DArray :
    ISamplable<Vector4, Vector2>,
    ILODSamplable<Vector4, Vector2>
{
    Image1DArray Image { get; }
}

[ImageInfo(Dim = Dim.Dim1D, Array = false, Depth = true, MultiSampled = false)]
public interface Image1DDepth
{
    Sampler1DDepth Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim1D, Array = false, Depth = true, MultiSampled = false)]
public interface Sampler1DDepth :
    ISamplable<Vector4, float>,
    ILODSamplable<Vector4, float>
{
    Image1DDepth Image { get; }
}

[ImageInfo(Dim = Dim.Dim1D, Array = true, Depth = true, MultiSampled = false)]
public interface Image1DDepthArray
{
    Sampler1DDepthArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim1D, Array = true, Depth = true, MultiSampled = false)]
public interface Sampler1DDepthArray :
    ISamplable<Vector4, Vector2>,
    ILODSamplable<Vector4, Vector2>
{
    Image1DDepthArray Image { get; }
}

[ImageInfo(Dim = Dim.Dim2D, Array = false, Depth = false, MultiSampled = false)]
public interface Image2D
{
    Sampler2D Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim2D, Array = false, Depth = false, MultiSampled = false)]
public interface Sampler2D :
    ISamplable<Vector4, Vector2>,
    ILODSamplable<Vector4, Vector2>,
    IGatherable<Vector4, Vector2>
{
    Image2D Image { get; }
}

[ImageInfo(Dim = Dim.Dim2D, Array = true, Depth = false, MultiSampled = false)]
public interface Image2DArray
{
    Sampler2DArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim2D, Array = true, Depth = false, MultiSampled = false)]
public interface Sampler2DArray :
    ISamplable<Vector4, Vector3>,
    ILODSamplable<Vector4, Vector3>,
    IGatherable<Vector4, Vector3>
{
    Image2DArray Image { get; }
}

[ImageInfo(Dim = Dim.Dim2D, Array = false, Depth = true, MultiSampled = false)]
public interface Image2DDepth
{
    Sampler2DDepth Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim2D, Array = false, Depth = true, MultiSampled = false)]
public interface Sampler2DDepth :
    ISamplable<Vector4, Vector2>,
    ILODSamplable<Vector4, Vector2>,
    IGatherable<Vector4, Vector2>
{
    Image2DDepth Image { get; }
}

[ImageInfo(Dim = Dim.Dim2D, Array = true, Depth = true, MultiSampled = false)]
public interface Image2DDepthArray
{
    Sampler2DDepthArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim2D, Array = true, Depth = true, MultiSampled = false)]
public interface Sampler2DDepthArray :
    ISamplable<Vector4, Vector3>,
    ILODSamplable<Vector4, Vector3>,
    IGatherable<Vector4, Vector3>
{
    Image2DDepthArray Image { get; }
}

[ImageInfo(Dim = Dim.Dim3D, Array = false, Depth = false, MultiSampled = false)]
public interface Image3D
{
    Sampler3D Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim3D, Array = false, Depth = false, MultiSampled = false)]
public interface Sampler3D :
    ISamplable<Vector4, Vector3>,
    ILODSamplable<Vector4, Vector3>
{
    Image3D Image { get; }
}

[ImageInfo(Dim = Dim.Dim3D, Array = true, Depth = false, MultiSampled = false)]
public interface Image3DArray
{
    Sampler3DArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim3D, Array = true, Depth = false, MultiSampled = false)]
public interface Sampler3DArray :
    ISamplable<Vector4, Vector4>,
    ILODSamplable<Vector4, Vector4>
{
    Image3DArray Image { get; }
}

[ImageInfo(Dim = Dim.Dim3D, Array = false, Depth = true, MultiSampled = false)]
public interface Image3DDepth
{
    Sampler3DDepth Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim3D, Array = false, Depth = true, MultiSampled = false)]
public interface Sampler3DDepth :
    ISamplable<Vector4, Vector3>,
    ILODSamplable<Vector4, Vector3>
{
    Image3DDepth Image { get; }
}

[ImageInfo(Dim = Dim.Dim3D, Array = true, Depth = true, MultiSampled = false)]
public interface Image3DDepthArray
{
    Sampler3DDepthArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Dim3D, Array = true, Depth = true, MultiSampled = false)]
public interface Sampler3DDepthArray :
    ISamplable<Vector4, Vector4>,
    ILODSamplable<Vector4, Vector4>
{
    Image3DDepthArray Image { get; }
}

[ImageInfo(Dim = Dim.Cube, Array = false, Depth = false, MultiSampled = false)]
public interface ImageCube
{
    SamplerCube Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Cube, Array = false, Depth = false, MultiSampled = false)]
public interface SamplerCube :
    ISamplable<Vector4, Vector3>,
    ILODSamplable<Vector4, Vector3>,
    IGatherable<Vector4, Vector3>
{
    ImageCube Image { get; }
}

[ImageInfo(Dim = Dim.Cube, Array = true, Depth = false, MultiSampled = false)]
public interface ImageCubeArray
{
    SamplerCubeArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Cube, Array = true, Depth = false, MultiSampled = false)]
public interface SamplerCubeArray :
    ISamplable<Vector4, Vector4>,
    ILODSamplable<Vector4, Vector4>,
    IGatherable<Vector4, Vector4>
{
    ImageCubeArray Image { get; }
}

[ImageInfo(Dim = Dim.Cube, Array = false, Depth = true, MultiSampled = false)]
public interface ImageCubeDepth
{
    SamplerCubeDepth Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Cube, Array = false, Depth = true, MultiSampled = false)]
public interface SamplerCubeDepth :
    ISamplable<Vector4, Vector3>,
    ILODSamplable<Vector4, Vector3>,
    IGatherable<Vector4, Vector3>
{
    ImageCubeDepth Image { get; }
}

[ImageInfo(Dim = Dim.Cube, Array = true, Depth = true, MultiSampled = false)]
public interface ImageCubeDepthArray
{
    SamplerCubeDepthArray Sampled(Sampler sampler);
}

[ImageInfo(Dim = Dim.Cube, Array = true, Depth = true, MultiSampled = false)]
public interface SamplerCubeDepthArray :
    ISamplable<Vector4, Vector4>,
    ILODSamplable<Vector4, Vector4>,
    IGatherable<Vector4, Vector4>
{
    ImageCubeDepthArray Image { get; }
}
