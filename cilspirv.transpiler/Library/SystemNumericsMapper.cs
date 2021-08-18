using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Transpiler;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using Mono.Cecil;

using static cilspirv.Library.ExternalMethodMapper;

namespace cilspirv.Library
{
    internal class SystemNumericsMapper : ITranspilerLibraryMapper
    {
        private static readonly SpirvVectorType SpirvVector2 = new SpirvVectorType()
        {
            ComponentType = new SpirvFloatingType() { Width = 32 },
            ComponentCount = 2
        };
        private static readonly SpirvVectorType SpirvVector3 = new SpirvVectorType()
        {
            ComponentType = new SpirvFloatingType() { Width = 32 },
            ComponentCount = 3
        };
        private static readonly SpirvVectorType SpirvVector4 = new SpirvVectorType()
        {
            ComponentType = new SpirvFloatingType() { Width = 32 },
            ComponentCount = 4
        };
        private static readonly SpirvVectorType SpirvQuaternion = SpirvVector4;
        private static readonly SpirvMatrixType SpirvMatrix3x2 = new SpirvMatrixType()
        {
            ColumnType = new SpirvVectorType()
            {
                ComponentType = new SpirvFloatingType() { Width = 32 },
                ComponentCount = 3
            },
            ColumnCount = 2
        };
        private static readonly SpirvMatrixType SpirvMatrix4x4 = new SpirvMatrixType()
        {
            ColumnType = new SpirvVectorType()
            {
                ComponentType = new SpirvFloatingType() { Width = 32 },
                ComponentCount = 4
            },
            ColumnCount = 4
        };

        private readonly ITranspilerLibraryMapper typeMapper = new ExternalTypeMapper()
        {
            { typeof(Vector2), SpirvVector2 },
            { typeof(Vector3), SpirvVector3 },
            { typeof(Vector4), SpirvVector4 },
            { typeof(Quaternion), SpirvQuaternion},
            { typeof(Matrix3x2), SpirvMatrix3x2},
            { typeof(Matrix4x4), SpirvMatrix4x4},
        };

        private readonly ExternalMethodMapper methodMapper = new ExternalMethodMapper()
        {
            {
                FullNameOfCtor<Vector2>(typeof(float)),
                CallSingleValueComposite(SpirvVector2)
            },
            {
                FullNameOfCtor<Vector2>(typeof(float), typeof(float)),
                CallOpCompositeConstruct(SpirvVector2)
            },
            {
                FullNameOfCtor<Vector3>(typeof(float)),
                CallSingleValueComposite(SpirvVector3)
            },
            {
                FullNameOfCtor<Vector3>(typeof(Vector2), typeof(float)),
                CallReconstructComposite(SpirvVector3)
            },
            {
                FullNameOfCtor<Vector3>(typeof(float), typeof(float), typeof(float)),
                CallOpCompositeConstruct(SpirvVector3)
            },
            {
                FullNameOfCtor<Vector4>(typeof(float)),
                CallSingleValueComposite(SpirvVector4)
            },
            {
                FullNameOfCtor<Vector4>(typeof(Vector3), typeof(float)),
                CallReconstructComposite(SpirvVector4)
            },
            {
                FullNameOfCtor<Vector4>(typeof(Vector2), typeof(float), typeof(float)),
                CallReconstructComposite(SpirvVector4)
            },
            {
                FullNameOfCtor<Vector4>(typeof(float), typeof(float), typeof(float), typeof(float)),
                CallOpCompositeConstruct(SpirvVector4)
            },
            {
                FullNameOf<Vector4>(nameof(Vector4.Transform), typeof(Vector4), typeof(Matrix4x4)),
                context => new[]
                {
                    new OpMatrixTimesVector()
                    {
                        Result = context.ResultID,
                        ResultType = context.IDOf(SpirvVector4),
                        Vector = context.Parameters[0].Item1,
                        Matrix = context.Parameters[1].Item1
                    }
                }
            }
        };

        public SystemNumericsMapper()
        {
            methodMapper.AddAllOperators<Vector2>(SpirvVector2);
            methodMapper.AddAllOperators<Vector3>(SpirvVector3);
            methodMapper.AddAllOperators<Vector4>(SpirvVector4);
        }

        public IMappedFromCILType? TryMapType(TypeReference ilTypeRef) => typeMapper.TryMapType(ilTypeRef);
        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => methodMapper.TryMapMethod(methodRef);
    }
}
