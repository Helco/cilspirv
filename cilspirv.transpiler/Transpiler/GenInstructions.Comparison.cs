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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var b = PopNumeric();
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
                var a = PopNumeric();
                var resultId = context.CreateID();
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
                var a = PopNumeric();
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
