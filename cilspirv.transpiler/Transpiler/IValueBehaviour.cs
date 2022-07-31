using System;
using System.Collections.Generic;
using cilspirv.Spirv.Ops;
using SpirvInstruction = cilspirv.Spirv.Instruction;

namespace cilspirv.Transpiler
{
    internal interface IValueBehaviour
    {
        // if Load/Store returns null, a standard OpLoad/OpStore is attempted using LoadAddress
        // if LoadAddress is not defined, ldflda is not supported

        IEnumerable<SpirvInstruction>? LoadAddress(ITranspilerValueContext context) { return null; }
        IEnumerable<SpirvInstruction>? Load(ITranspilerValueContext context) { return null; }
        IEnumerable<SpirvInstruction>? Store(ITranspilerValueContext context, ValueStackEntry value) { return null; }

        IEnumerable<SpirvInstruction> LoadIndirect(ITranspilerValueContext context)
        {
            if (context.Parent is not ValueStackEntry pointer)
                throw new InvalidOperationException("LoadObject source is not a value");
            if (pointer.Type is not SpirvPointerType pointerType)
                throw new InvalidOperationException("LoadObject source is not a pointer value");

            var resultId = context.CreateID();
            yield return new OpLoad()
            {
                Result = resultId,
                ResultType = context.IDOf(pointerType.Type!),
                Pointer = pointer.ID
            };
            context.Result = new ValueStackEntry(resultId, pointerType.Type!);
        }

        IEnumerable<SpirvInstruction> StoreIndirect(ITranspilerValueContext context, StackEntry entry)
        {
            if (entry is not ValueStackEntry value)
                throw new InvalidOperationException("StoreObject entry is not a value");
            if (context.Parent is not ValueStackEntry pointer)
                throw new InvalidOperationException("StoreObjectg target is not a value");
            if (pointer.Type is not SpirvPointerType)
                throw new InvalidOperationException("StoreObject destination is not a pointer value");

            yield return new OpStore()
            {
                Pointer = pointer.ID,
                Object = value.ID
            };
        }
    }
}
