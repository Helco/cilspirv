using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace cilspirv.Transpiler
{
    internal class TranspilerReferenceMapper : ITranspilerLibraryMapper
    {
        private readonly TranspilerLibrary library;

        public TranspilerReferenceMapper(TranspilerLibrary library) => this.library = library;

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;

        public IMappedFromCILType? TryMapType(TypeReference ilTypeRef)
        {
            if (ilTypeRef is not ByReferenceType byRefType)
                return null;
            var elementType = library.TryMapType(byRefType.ElementType);
            if (elementType == null)
                return null;
            return elementType;
        }
    }
}
