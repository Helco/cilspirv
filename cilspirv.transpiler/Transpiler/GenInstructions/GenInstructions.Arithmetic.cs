using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;

namespace cilspirv.Transpiler
{
    partial class GenInstructions
    {
        private SpirvNumericType GetCoercedNumeric(SpirvNumericType a, SpirvNumericType b)
        {
            if (a == b)
                return a;

            var aFloat = a as SpirvFloatingType;
            var bFloat = b as SpirvFloatingType;
            var aInt = a as SpirvIntegerType;
            var bInt = b as SpirvIntegerType;

            if ((aFloat ?? bFloat) != null)
                return new SpirvFloatingType() { Width = Math.Max(aFloat?.Width ?? 0, bFloat?.Width ?? 0) };
            if (aInt == null || bInt == null)
                throw new NotSupportedException("Unknown numeric types");
            if (aInt.Width > bInt.Width)
                return aInt;
            if (bInt.Width > aInt.Width)
                return bInt;
            if (aInt.Width == 64)
                throw new InvalidOperationException("Invalid implicit cast from signed/unsigned int64");
            return new SpirvIntegerType() { Width = aInt.Width * 2, IsSigned = true };
        }

        private ID NumericCast(ValueStackEntry value, SpirvNumericType target)
        {
            if (value.Type == target)
                return value.ID;
            ID resultId;
            var resultTypeId = context.IDOf(target);

            switch (value.Type, target)
            {
                case (SpirvIntegerType fromInt, SpirvIntegerType toInt) when fromInt.IsSigned == toInt.IsSigned:
                    resultId = CoerceIntWidth(value.ID, fromInt, toInt);
                    break;
                case (SpirvIntegerType fromInt, SpirvIntegerType toInt):
                    Add(new OpBitcast()
                    {
                        Result = resultId = context.CreateID(),
                        ResultType = resultTypeId,
                        Operand = CoerceIntWidth(value.ID, fromInt, toInt)
                    });
                    break;
                case (SpirvFloatingType floatType, SpirvFloatingType toFloat):
                    Add(new OpFConvert()
                    {
                        Result = resultId = context.CreateID(),
                        ResultType = resultTypeId,
                        FloatValue = value.ID
                    });
                    break;
                case (SpirvIntegerType fromInt, SpirvFloatingType toFloat) when fromInt.IsSigned:
                    Add(new OpConvertSToF()
                    {
                        Result = resultId = context.CreateID(),
                        ResultType = resultTypeId,
                        SignedValue = value.ID
                    });
                    break;
                case (SpirvIntegerType fromInt, SpirvFloatingType toFloat):
                    Add(new OpConvertUToF()
                    {
                        Result = resultId = context.CreateID(),
                        ResultType = resultTypeId,
                        UnsignedValue = value.ID
                    });
                    break;
                case (SpirvFloatingType fromFloat, SpirvIntegerType toInt) when toInt.IsSigned:
                    Add(new OpConvertFToS()
                    {
                        Result = resultId = context.CreateID(),
                        ResultType = resultTypeId,
                        FloatValue = value.ID
                    });
                    break;
                case (SpirvFloatingType fromFloat, SpirvIntegerType toInt):
                    Add(new OpConvertFToU()
                    {
                        Result = resultId = context.CreateID(),
                        ResultType = resultTypeId,
                        FloatValue = value.ID
                    });
                    break;
                default: throw new NotSupportedException("Unsupported numeric cast");
            };
            return resultId;
        }

        private ID CoerceIntWidth(ID value, SpirvIntegerType fromInt, SpirvIntegerType toInt)
        {
            if (fromInt.Width == toInt.Width)
                return value;
            var resultId = context.CreateID();
            var resultTypeId = context.IDOf(
                new SpirvIntegerType() { Width = toInt.Width, IsSigned = fromInt.IsSigned });
            Add(toInt.IsSigned
                ? new OpSConvert()
                {
                    Result = resultId,
                    ResultType = resultTypeId,
                    SignedValue = value,
                }
                : new OpUConvert()
                {
                    Result = resultId,
                    ResultType = resultTypeId,
                    UnsignedValue = value
                });
            return resultId;
        }

        private ValueStackEntry PopNumeric()
        {
            var result = PopValue();
            if (result.Type is not SpirvNumericType)
                throw new InvalidOperationException("Top of stack is not a numeric value");
            return result;
        }

        private void ConvertToSigned(int width) => ConvertToNumeric(new SpirvIntegerType() { Width = width, IsSigned = true });
        private void ConvertToUnsigned(int width) => ConvertToNumeric(new SpirvIntegerType() { Width = width, IsSigned = false });
        private void ConvertToFloating(int width) => ConvertToNumeric(new SpirvFloatingType() { Width = width });
        private void ConvertToNumeric(SpirvNumericType targetType)
        {
            var value = PopNumeric();
            var resultId = NumericCast(value, targetType);
            Stack.Add(new ValueStackEntry(resultId, targetType));
        }

        private void Addition()
        {
            var b = PopNumeric();
            var a = PopNumeric();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(type is SpirvIntegerType
                ? new OpIAdd()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                }
                : new OpFAdd()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void Subtraction()
        {
            var b = PopNumeric();
            var a = PopNumeric();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(type is SpirvIntegerType
                ? new OpISub()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                }
                : new OpFSub()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void Multiplication()
        {
            var b = PopNumeric();
            var a = PopNumeric();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(type is SpirvIntegerType
                ? new OpIMul()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                }
                : new OpFMul()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void Division()
        {
            var b = PopNumeric();
            var a = PopNumeric();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(type is SpirvIntegerType intType
                ? intType.IsSigned ? new OpSDiv()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                }
                : new OpUDiv()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                }
                : new OpFAdd()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void Remainder()
        {
            // TODO: Test signed and floating behavior
            var b = PopNumeric();
            var a = PopNumeric();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(type is SpirvIntegerType intType
                ? intType.IsSigned ? new OpSMod()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                }
                : new OpUMod()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                }
                : new OpFMod()
                {
                    Result = resultId,
                    ResultType = context.IDOf(type),
                    Operand1 = NumericCast(a, type),
                    Operand2 = NumericCast(b, type)
                });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void Negate()
        {
            var a = PopNumeric();
            var resultId = context.CreateID();
            var resultType = a.Type is SpirvIntegerType intType && !intType.IsSigned
                ? new SpirvIntegerType() { Width = intType.Width, IsSigned = true }
                : a.Type;

            Add(a.Type is SpirvIntegerType
                ? new OpSNegate()
                {
                    Result = resultId,
                    ResultType = context.IDOf(resultType),
                    Operand = a.ID
                }
                : new OpFNegate()
                {
                    Result = resultId,
                    ResultType = context.IDOf(resultType),
                    Operand = a.ID
                });
            Stack.Add(new ValueStackEntry(resultId, resultType));
        }
    }
}
