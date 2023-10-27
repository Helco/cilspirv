using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Transpiler;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using System.Collections.Immutable;

namespace cilspirv.Library
{
    partial class ExternalMethodMapper
    {
        public void AddAllOperators<T>(SpirvType meType)
        {
            var opMethods = typeof(T)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(m => m.Name.StartsWith("op_") && Enum.TryParse<OperatorKind>(m.Name.Substring(3), out _));

            foreach (var method in opMethods)
            {
                var operatorKind = (OperatorKind)Enum.Parse(typeof(OperatorKind), method.Name.Substring(3));
                var callMethod = (operatorKind, meType) switch
                {
                    (OperatorKind.Addition, SpirvVectorType vectorType) => CallVectorAdd(vectorType),
                    (OperatorKind.Subtraction, SpirvVectorType vectorType) => CallVectorSub(vectorType),
                    (OperatorKind.Multiply, SpirvVectorType vectorType) => CallVectorMul(vectorType),
                    (OperatorKind.Division, SpirvVectorType vectorType) => CallVectorDiv(vectorType),
                    (OperatorKind.Modulus, SpirvVectorType vectorType) => CallVectorMod(vectorType),
                    _ => null
                };
                if (callMethod != null)
                    methods.Add(method.FullCilspirvName(), callMethod);
            }
        }

        private static IEnumerable<Instruction> PrepareArithmetic(SpirvVectorType meType, ITranspilerMethodContext context, out ID op1, out ID op2)
        {
            if (context.Parameters.Count != 2 || context.This != null)
                throw new InvalidOperationException("Arithmetic operation expects two parameters");
            SpirvType op1Type, op2Type;
            (op1, op1Type) = context.Parameters[0];
            (op2, op2Type) = context.Parameters[1];

            return new[]
            {
                ExpandToVector(ref op1, op1Type),
                ExpandToVector(ref op2, op2Type)
            }.Where(i => i != null);

            Instruction ExpandToVector(ref ID op, SpirvType type)
            {
                if (type == meType)
                    return null!;
                if (type != meType.ComponentType)
                    throw new InvalidOperationException($"Unexpected arithmetic operand type: {op1Type}");
                var elemId = op;
                return new OpCompositeConstruct()
                {
                    Result = op = context.CreateID(),
                    ResultType = context.IDOf(meType),
                    Constituents = Enumerable.Repeat(elemId, meType.ComponentCount).ToImmutableArray()
                };
            }
        }

        public static GenerateCallDelegate CallVectorAdd(SpirvVectorType meType) => context =>
            PrepareArithmetic(meType, context, out var op1, out var op2).Append(
                meType.ComponentType is SpirvIntegerType
                ? new OpIAdd()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                }
                : new OpFAdd()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                });

        public static GenerateCallDelegate CallVectorSub(SpirvVectorType meType) => context =>
            PrepareArithmetic(meType, context, out var op1, out var op2).Append(
                meType.ComponentType is SpirvIntegerType
                ? new OpISub()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                }
                : new OpFSub()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                });

        public static GenerateCallDelegate CallVectorMul(SpirvVectorType meType) => context =>
            PrepareArithmetic(meType, context, out var op1, out var op2).Append(
                meType.ComponentType is SpirvIntegerType
                ? new OpIMul()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                }
                : new OpFMul()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                });

        public static GenerateCallDelegate CallVectorDiv(SpirvVectorType meType) => context =>
            PrepareArithmetic(meType, context, out var op1, out var op2).Append(
                meType.ComponentType is SpirvIntegerType intType
                ? intType.IsSigned ? new OpSDiv()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                }
                : new OpUDiv()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                }
                : new OpFDiv()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                });

        public static GenerateCallDelegate CallVectorMod(SpirvVectorType meType) => context =>
            PrepareArithmetic(meType, context, out var op1, out var op2).Append(
                meType.ComponentType is SpirvIntegerType intType
                ? intType.IsSigned ? new OpSMod()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                }
                : new OpUMod()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                }
                : new OpFMod()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Operand1 = op1,
                    Operand2 = op2
                });

        public static GenerateCallDelegate CallVectorDot(SpirvVectorType meType) => context =>
            PrepareArithmetic(meType, context, out var op1, out var op2).Append(
                new OpDot()
                {
                    Result = context.ResultID,
                    ResultType = context.IDOf(meType),
                    Vector1 = op1,
                    Vector2 = op2
                });
    }
}
