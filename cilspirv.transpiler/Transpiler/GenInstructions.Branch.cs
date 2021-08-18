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
            private void Call(MethodReference methodRef, bool isCtor)
            {
                // TODO: Coercing
                var callMethod = Library.MapMethod(methodRef);
                var paramCount = methodRef.Parameters.Count;
                var stackCount = paramCount + (!isCtor && methodRef.HasThis ? 1 : 0);

                if (Stack.Count < stackCount)
                    throw new InvalidOperationException($"Stack does not have all parameters");
                var thiz = Stack[Stack.Count - stackCount] as ValueStackEntry; // calling a method on a VarGroup still has a null this pointer
                var parameters = Stack
                    .TakeLast(paramCount)
                    .Select((e, i) => e as ValueStackEntry ?? throw new InvalidOperationException($"Parameter {i} is not a value entry"))
                    .Select(e => (e.ID, e.Type))
                    .ToArray();
                Block.Instructions.AddRange(callMethod(this, parameters, out var resultId));

                Stack.RemoveRange(Stack.Count - stackCount, stackCount);
                if (resultId != null)
                {
                    var returnType = (SpirvType)Library.MapType(isCtor
                        ? methodRef.DeclaringType
                        : methodRef.ReturnType);
                    Stack.Add(new ValueStackEntry(methodRef, resultId.Value, returnType));
                }
            }

            private void Return()
            {
                // TODO: Check return type and coerce
                if (Function.ReturnType is SpirvVoidType)
                    Add(new OpReturn());
                else
                {
                    if (Pop() is not ValueStackEntry returnValue)
                        throw new InvalidOperationException("Top of stack is not SPIRV entry");
                    Add(new OpReturnValue()
                    {
                        Value = returnValue.ID
                    });
                }
                LeaveBlock();
            }

            private void Branch(ILInstruction target)
            {
                if (!blocks.TryGetValue(target.Offset, out var targetBlock))
                    blocks[target.Offset] = targetBlock = new BlockInfo(Stack.ToList());
                Add(new OpBranch()
                {
                    TargetLabel = context.IDOf(targetBlock.block)
                });
                LeaveBlock();
            }
        }
    }
}
