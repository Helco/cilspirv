using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using Mono.Cecil;

namespace cilspirv.Transpiler
{
    internal class TranspilerInternalMethodMapper : ITranspilerLibraryMapper
    {
        private readonly TranspilerLibrary library;
        private readonly Dictionary<string, GenerateCallDelegate> methods = new Dictionary<string, GenerateCallDelegate>();

        public TranspilerInternalMethodMapper(TranspilerLibrary library) => this.library = library;

        public IMappedFromCILType? TryMapType(TypeReference ilTypeRef) => null;

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef)
        {
            if (methods.TryGetValue(methodRef.FullName, out var mapped))
                return mapped;

            var function = library.TryMapInternalMethod(methodRef.Resolve(), isEntryPoint: false);
            if (function == null)
                return null;

            mapped = GenerateCallFor(function);
            methods.Add(methodRef.FullName, mapped);
            return mapped;
        }

        private GenerateCallDelegate GenerateCallFor(TranspilerFunction function) => context => new[]
        {
            new OpFunctionCall()
            {
                Result = context.ResultID,
                ResultType = context.IDOf(function.ReturnType),
                Function = context.IDOf(function),
                Arguments = context.Parameters.Select(p => p.id).ToImmutableArray()
            }
        };
    }
}
