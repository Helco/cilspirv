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
        private ValueStackEntry PopInteger()
        {
            var result = PopValue();
            if (result.Type is not SpirvIntegerType)
                throw new InvalidOperationException("Top of stack is not a integer value");
            return result;
        }

        private void BitwiseAnd()
        {
            var b = PopInteger();
            var a = PopInteger();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(new OpBitwiseAnd()
            {
                Result = resultId,
                ResultType = context.IDOf(type),
                Operand1 = NumericCast(a, type),
                Operand2 = NumericCast(a, type),
            });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void BitwiseOr()
        {
            var b = PopInteger();
            var a = PopInteger();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(new OpBitwiseOr()
            {
                Result = resultId,
                ResultType = context.IDOf(type),
                Operand1 = NumericCast(a, type),
                Operand2 = NumericCast(a, type),
            });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void BitwiseXor()
        {
            var b = PopInteger();
            var a = PopInteger();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(new OpBitwiseAnd()
            {
                Result = resultId,
                ResultType = context.IDOf(type),
                Operand1 = NumericCast(a, type),
                Operand2 = NumericCast(a, type),
            });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void ShiftLeftLogical()
        {
            var b = PopInteger();
            var a = PopInteger();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(new OpShiftLeftLogical()
            {
                Result = resultId,
                ResultType = context.IDOf(type),
                Base = NumericCast(a, type),
                Shift = NumericCast(a, type),
            });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void ShiftRightLogical()
        {
            var b = PopInteger();
            var a = PopInteger();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(new OpShiftLeftLogical()
            {
                Result = resultId,
                ResultType = context.IDOf(type),
                Base = NumericCast(a, type),
                Shift = NumericCast(a, type),
            });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void ShiftRightArithmetic()
        {
            var b = PopInteger();
            var a = PopInteger();
            var type = GetCoercedNumeric((SpirvNumericType)a.Type, (SpirvNumericType)b.Type);
            var resultId = context.CreateID();
            Add(new OpShiftLeftLogical()
            {
                Result = resultId,
                ResultType = context.IDOf(type),
                Base = NumericCast(a, type),
                Shift = NumericCast(a, type),
            });
            Stack.Add(new ValueStackEntry(resultId, type));
        }

        private void BitwiseNot()
        {
            var a = PopInteger();
            var resultId = context.CreateID();
            Add(new OpNot()
            {
                Result = resultId,
                ResultType = context.IDOf(a.Type),
                Operand = a.ID
            });
            Stack.Add(new ValueStackEntry(resultId, a.Type));
        }
    }
}
