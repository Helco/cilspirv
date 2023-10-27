using System;
using cilspirv.Transpiler;
using cilspirv.Spirv;
using Mono.Cecil;

namespace cilspirv.Library
{
    internal class BuiltinTypeMapper : NullTranspilerLibraryMapper
    {
        private readonly ITranspilerLibraryMapper typeMapper = new ExternalTypeMapper()
        {
            { typeof(sbyte), new SpirvIntegerType() { Width = 8, IsSigned = true } },
            { typeof(byte), new SpirvIntegerType() { Width = 8, IsSigned = false } },
            { typeof(short), new SpirvIntegerType() { Width = 16, IsSigned = true } },
            { typeof(ushort), new SpirvIntegerType() { Width = 16, IsSigned = false } },
            { typeof(int), new SpirvIntegerType() { Width = 32, IsSigned = true } },
            { typeof(uint), new SpirvIntegerType() { Width = 32, IsSigned = false } },
            { typeof(long), new SpirvIntegerType() { Width = 64, IsSigned = true } },
            { typeof(ulong), new SpirvIntegerType() { Width = 64, IsSigned = false } },
            { typeof(float), new SpirvFloatingType() { Width = 32 } },
            { typeof(double), new SpirvFloatingType() { Width = 64 } },
            { typeof(bool), new SpirvBooleanType() },
            { typeof(void), new SpirvVoidType() }
        };

        public override IMappedFromCILType? TryMapType(TypeReference ilTypeRef) => typeMapper.TryMapType(ilTypeRef);
    }
}
