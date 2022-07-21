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
    partial class Transpiler
    {
        partial class GenInstructions
        {
            private void PushCompareEqual() =>
                Stack.Add(new ValueStackEntry(CompareEqual(), new SpirvBooleanType()));

            private void PushCompareGreater() =>
                Stack.Add(new ValueStackEntry(CompareGreater(), new SpirvBooleanType()));

            private void PushCompareGreaterUnordered() =>
                Stack.Add(new ValueStackEntry(CompareGreaterUnordered(), new SpirvBooleanType()));

            private void PushCompareLess() =>
                Stack.Add(new ValueStackEntry(CompareLess(), new SpirvBooleanType()));

            private void PushCompareLessUnordered() =>
                Stack.Add(new ValueStackEntry(CompareLessUnordered(), new SpirvBooleanType()));

            private ID CompareEqual()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? new OpIEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFOrdEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareNotEqual()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? new OpINotEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFOrdNotEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareGreater()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? intType.IsSigned ? new OpSGreaterThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpUGreaterThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFOrdGreaterThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareGreaterUnordered()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? new OpUGreaterThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFUnordGreaterThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareGreaterOrEqual()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? intType.IsSigned ? new OpSGreaterThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpUGreaterThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFOrdGreaterThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareGreaterOrEqualUnordered()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? new OpUGreaterThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFUnordGreaterThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareLess()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? intType.IsSigned ? new OpSLessThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpULessThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFOrdLessThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareLessUnordered()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? new OpULessThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFUnordLessThan()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareLessOrEqual()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? intType.IsSigned ? new OpSLessThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpULessThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFOrdLessThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareLessOrEqualUnordered()
            {
                var b = PopNumeric();
                var a = PopNumeric();
                var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
                var resultId = context.CreateID();
                Add(type is SpirvIntegerType intType
                    ? new OpULessThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    }
                    : new OpFUnordLessThanEqual()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(new SpirvBooleanType()),
                        Operand1 = NumericCast(a, type),
                        Operand2 = NumericCast(b, type)
                    });
                return resultId;
            }

            private ID CompareFalsy()
            {
                ID resultId;
                var a = PopValue();
                if (a.Type is SpirvBooleanType)
                {
                    resultId = context.CreateID();
                    Add(new OpLogicalNot()
                    {
                        Result = resultId,
                        ResultType = context.IDOf(a.Type),
                        Operand = a.ID
                    });
                    return resultId;
                }

                if (a.Type is not SpirvNumericType)
                    throw new InvalidOperationException("Invalid type to compare falsy");
                resultId = context.CreateID();
                Add(new OpIEqual()
                {
                    Result = resultId,
                    ResultType = context.IDOf(new SpirvBooleanType()),
                    Operand1 = a.ID,
                    Operand2 = context.IDOf(new TranspilerNumericConstant(
                        ImmutableArray.Create((LiteralNumber)0),
                        (SpirvNumericType)a.Type))
                });
                return resultId;
            }

            private ID CompareTruthy()
            {
                var a = PopValue();
                if (a.Type is SpirvBooleanType)
                    return a.ID;
                if (a.Type is not SpirvNumericType)
                    throw new InvalidOperationException("Invalid type to compare falsy");
                var resultId = context.CreateID();
                Add(new OpINotEqual()
                {
                    Result = resultId,
                    ResultType = context.IDOf(new SpirvBooleanType()),
                    Operand1 = a.ID,
                    Operand2 = context.IDOf(new TranspilerNumericConstant(
                        ImmutableArray.Create((LiteralNumber)0),
                        (SpirvNumericType)a.Type))
                });
                return resultId;
            }
        }
    }
}
