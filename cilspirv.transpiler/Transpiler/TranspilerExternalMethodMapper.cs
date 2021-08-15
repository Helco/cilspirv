﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using Mono.Cecil;

namespace cilspirv.Transpiler
{
    internal class TranspilerExternalMethodMapper : ITranspilerLibraryMapper, IEnumerable<KeyValuePair<string, GenerateCallDelegate>>
    {
        private readonly Dictionary<string, GenerateCallDelegate> methods = new Dictionary<string, GenerateCallDelegate>();

        public TranspilerType? TryMapType(TypeReference ilTypeRef) => null;

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) =>
            methods.TryGetValue(methodRef.FullName, out var method)
            ? method
            : null;

        public void Add(string fullMethodName, GenerateCallDelegate method) => methods.Add(fullMethodName, method);
        public IEnumerator<KeyValuePair<string, GenerateCallDelegate>> GetEnumerator() => methods.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => methods.GetEnumerator();

        public static string FullNameOf<T>(string localMethodName)
        {
            var methodInfos = typeof(T).GetMethods().Where(m => m.Name == localMethodName);
            if (!methodInfos.Any())
                throw new ArgumentException($"Could not find method \"{localMethodName}\" on type {typeof(T).FullName}");
            if (methodInfos.Count() > 1)
                throw new InvalidOperationException($"Method \"{localMethodName}\" on type {typeof(T).FullName} is ambiguous");
            return methodInfos.Single().FullName();
        }

        public static string FullNameOf<T>(string localMethodName, params Type[] parameters)
        {
            var methodInfo = typeof(T).GetMethod(localMethodName, parameters);
            if (methodInfo == null)
                throw new ArgumentException($"Could not find \"{localMethodName}({string.Join(",", parameters.Select(p => p.FullName))})\" on type {typeof(T).FullName}");
            return methodInfo.FullName();
        }

        public static string FullNameOfCtor<T>(params Type[] parameters)
        {
            var ctorInfo = typeof(T).GetConstructor(parameters);
            if (ctorInfo == null)
                throw new ArgumentException($"Could not find \".ctor({string.Join(",", parameters.Select(p => p.FullName))})\" on type {typeof(T).FullName}");
            return ctorInfo.FullName();
        }

        public static GenerateCallDelegate CallOpCompositeConstruct(SpirvType resultType) => (ITranspilerMethodContext context, IReadOnlyList<(ID id, SpirvType)> parameters, out ID? resultId) => new[]
        {
            new OpCompositeConstruct()
            {
                Result = (resultId = context.CreateID()).Value,
                ResultType = context.IDOf(resultType),
                Constituents = parameters.Select(p => p.id).ToImmutableArray()
            }
        };
    }
}