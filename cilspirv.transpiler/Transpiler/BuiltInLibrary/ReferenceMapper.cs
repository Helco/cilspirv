using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace cilspirv.Transpiler.BuiltInLibrary
{
    internal class ReferenceMapper : NullTranspilerLibraryMapper
    {
        private readonly TranspilerLibrary library;

        public ReferenceMapper(TranspilerLibrary library) => this.library = library;

        public override GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;

        public override IMappedFromCILType? TryMapType(TypeReference ilTypeRef)
        {
            if (ilTypeRef is not ByReferenceType byRefType)
                return null;
            var elementType = library.TryMapType(byRefType.ElementType);
            if (elementType == null)
                return null;
            return new MappedFromRefCILType(elementType);
        }
    }
}

namespace cilspirv.Transpiler
{
    internal record MappedFromRefCILType(IMappedFromCILType ElementType) : IMappedFromCILType
    {
    }
}
