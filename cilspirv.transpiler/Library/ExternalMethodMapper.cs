using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler;
using Mono.Cecil;

namespace cilspirv.Library
{
    internal partial class ExternalMethodMapper : NullTranspilerLibraryMapper, IEnumerable<KeyValuePair<string, GenerateCallDelegate>>
    {
        private readonly Dictionary<string, GenerateCallDelegate> methods = new Dictionary<string, GenerateCallDelegate>();

        public override GenerateCallDelegate? TryMapMethod(MethodReference methodRef) =>
            methods.TryGetValue(methodRef.FullCilspirvName(), out var method)
            ? method
            : null;

        public void Add(string fullMethodName, GenerateCallDelegate method) => methods.Add(fullMethodName, method);
        public IEnumerator<KeyValuePair<string, GenerateCallDelegate>> GetEnumerator() => methods.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => methods.GetEnumerator();

        public static string FullNameOf<T>(string localMethodName) => FullNameOf(typeof(T), localMethodName);
        public static string FullNameOf(Type type, string localMethodName)
        {
            var methodInfos = type.GetMethods().Where(m => m.Name == localMethodName);
            if (!methodInfos.Any())
                throw new ArgumentException($"Could not find method \"{localMethodName}\" on type {type.FullName}");
            if (methodInfos.Count() > 1)
                throw new InvalidOperationException($"Method \"{localMethodName}\" on type {type.FullName} is ambiguous");
            return methodInfos.Single().FullCilspirvName();
        }

        public static string FullNameOf<T>(string localMethodName, params Type[] parameters)
        {
            var methodInfo = typeof(T).GetMethod(localMethodName, parameters);
            if (methodInfo == null)
                throw new ArgumentException($"Could not find \"{localMethodName}({string.Join(",", parameters.Select(p => p.FullName))})\" on type {typeof(T).FullName}");
            return methodInfo.FullCilspirvName();
        }

        public static string FullNameOfCtor<T>(params Type[] parameters)
        {
            var ctorInfo = typeof(T).GetConstructor(parameters);
            if (ctorInfo == null)
                throw new ArgumentException($"Could not find \".ctor({string.Join(",", parameters.Select(p => p.FullName))})\" on type {typeof(T).FullName}");
            return ctorInfo.FullCilspirvName();
        }

        public enum OperatorKind
        {
            UnaryPlus,
            UnaryNegation,
            Increment,
            Decrement,
            LogicalNot,
            Addition,
            Subtraction,
            Multiply,
            Division,
            BitwiseAnd,
            BitwiseOr,
            ExclusiveOr,
            OnesComplement,
            Equality,
            Inequality,
            LessThan,
            GreaterThan,
            LessThanOrEqual,
            GreaterThanOrEqual,
            LeftShift,
            RightShift,
            Modulus,
            Implicit,
            Explicit,
            True,
            False
        }
        public static string FullNameOfOp<T>(OperatorKind op, params Type[] parameters) =>
            FullNameOf<T>($"op_{op}", parameters);

        public static GenerateCallDelegate CallOpCompositeConstruct(SpirvType resultType) => context => new[]
        {
            new OpCompositeConstruct()
            {
                Result = context.ResultID,
                ResultType = context.IDOf(resultType),
                Constituents = context.Parameters.Select(p => p.id).ToImmutableArray()
            }
        };

        public static GenerateCallDelegate CallSingleValueComposite(SpirvVectorType resultType) => context => new[]
        {
            new OpCompositeConstruct()
            {
                Result = context.ResultID,
                ResultType = context.IDOf(resultType),
                Constituents = Enumerable.Repeat(context.Parameters.Single().id, resultType.ComponentCount).ToImmutableArray()
            }
        };

        public static GenerateCallDelegate CallReconstructComposite(SpirvVectorType resultType) => context =>
        {
            int additionalComponents = context.Parameters.Count - 1;
            int originalComponents = resultType.ComponentCount - additionalComponents;
            ID componentID = context.IDOf(resultType.ComponentType ?? throw new ArgumentNullException("Component type is not set"));
            return Generate();

            IEnumerable<Instruction> Generate()
            {
                var originalIDs = new ID[originalComponents];
                for (int i = 0; i < originalComponents; i++)
                    yield return new OpCompositeExtract()
                    {
                        Result = originalIDs[i] = context.CreateID(),
                        ResultType = componentID,
                        Composite = context.Parameters.First().id,
                        Indexes = ImmutableArray.Create<LiteralNumber>(i)
                    };

                yield return new OpCompositeConstruct()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(resultType),
                    Constituents = originalIDs
                        .Concat(context.Parameters.Skip(1).Select(p => p.id))
                        .ToImmutableArray()
                };
            }
        };
    }
}
