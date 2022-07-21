using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Declarations;
using Mono.Cecil;

namespace cilspirv.Transpiler
{
    internal class InternalMethodMapper : ITranspilerLibraryMapper
    {
        private readonly TranspilerLibrary library;
        private readonly Dictionary<string, GenerateCallDelegate> methods = new Dictionary<string, GenerateCallDelegate>();

        public InternalMethodMapper(TranspilerLibrary library) => this.library = library;

        public IMappedFromCILType? TryMapType(TypeReference ilTypeRef) => null;

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef)
        {
            if (methods.TryGetValue(methodRef.FullName, out var mapped))
                return mapped;

            var methodDef = methodRef.Resolve();
            if (IsKillMethod(methodDef))
                return GenerateKill;

            var function = library.TryMapInternalMethod(methodDef, isEntryPoint: false);
            if (function == null)
                return null;

            mapped = GenerateCallFor(function);
            methods.Add(methodRef.FullName, mapped);
            return mapped;
        }

        private GenerateCallDelegate GenerateCallFor(Function function) => context => new[]
        {
            new OpFunctionCall()
            {
                Result = context.ResultID,
                ResultType = context.IDOf(function.ReturnType),
                Function = context.IDOf(function),
                Arguments = context.Parameters.Select(p => p.id).ToImmutableArray()
            }
        };

        private bool IsKillMethod(MethodDefinition methodDef) => methodDef
            .CustomAttributes
            .Select(attr => attr.AttributeType)
            .Any(typeRef =>
                typeRef.Name == nameof(DoesNotReturnAttribute) || // only local name 
                typeRef.FullName == typeof(Library.KillAttribute).FullName);

        private IEnumerable<Instruction> GenerateKill(ITranspilerContext context) => new[]
        {
            new OpKill()
        };
    }
}
