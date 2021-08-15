using System;
using System.Numerics;
using cilspirv.Transpiler;
using cilspirv.Spirv;
using Mono.Cecil;

using static cilspirv.Transpiler.TranspilerExternalMethodMapper;

namespace cilspirv.Library
{
    internal class SystemNumericsMapper : ITranspilerLibraryMapper
    {
        private static readonly SpirvType SpirvVector2 = new SpirvVectorType()
        {
            ComponentType = new SpirvFloatingType() { Width = 32 },
            ComponentCount = 2
        };
        private static readonly SpirvType SpirvVector3 = new SpirvVectorType()
        {
            ComponentType = new SpirvFloatingType() { Width = 32 },
            ComponentCount = 3
        };
        private static readonly SpirvType SpirvVector4 = new SpirvVectorType()
        {
            ComponentType = new SpirvFloatingType() { Width = 32 },
            ComponentCount = 4
        };
        private static readonly SpirvType SpirvQuaternion = SpirvVector4;
        private static readonly SpirvType SpirvMatrix3x2 = new SpirvMatrixType()
        {
            ColumnType = new SpirvVectorType()
            {
                ComponentType = new SpirvFloatingType() { Width = 32 },
                ComponentCount = 3
            },
            ColumnCount = 2
        };
        private static readonly SpirvType SpirvMatrix4x4 = new SpirvMatrixType()
        {
            ColumnType = new SpirvVectorType()
            {
                ComponentType = new SpirvFloatingType() { Width = 32 },
                ComponentCount = 4
            },
            ColumnCount = 4
        };

        private readonly ITranspilerLibraryMapper typeMapper = new TranspilerExternalTypeMapper()
        {
            { typeof(Vector2), SpirvVector2 },
            { typeof(Vector3), SpirvVector3 },
            { typeof(Vector4), SpirvVector4 },
            { typeof(Quaternion), SpirvQuaternion},
            { typeof(Matrix3x2), SpirvMatrix3x2},
            { typeof(Matrix4x4), SpirvMatrix4x4},
        };

        private readonly ITranspilerLibraryMapper methodMapper = new TranspilerExternalMethodMapper()
        {
            {
                FullNameOfCtor<Vector2>(typeof(float), typeof(float)),
                CallOpCompositeConstruct(SpirvVector4)
            },
            {
                FullNameOfCtor<Vector3>(typeof(float), typeof(float), typeof(float)),
                CallOpCompositeConstruct(SpirvVector4)
            },
            {
                FullNameOfCtor<Vector4>(typeof(float), typeof(float), typeof(float), typeof(float)),
                CallOpCompositeConstruct(SpirvVector4)
            }
        };

        public TranspilerType? TryMapType(TypeReference ilTypeRef) => typeMapper.TryMapType(ilTypeRef);
        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => methodMapper.TryMapMethod(methodRef);
    }
}
