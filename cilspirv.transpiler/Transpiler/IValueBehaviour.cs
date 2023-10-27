using System;
using System.Collections.Generic;
using cilspirv.Spirv.Ops;
using SpirvInstruction = cilspirv.Spirv.Instruction;

namespace cilspirv.Transpiler
{
    internal interface ITranspilerValueContext : ITranspilerContext
    {

        StackEntry Parent { get; } // will throw if no applicable parent exists (e.g. variable access)
        StackEntry Result { set; }
    }

    internal interface IValueBehaviour
    {
        // if Load/Store returns null, a standard OpLoad/OpStore is attempted using LoadAddress
        // if LoadAddress is not defined, ldflda is not supported

        IEnumerable<SpirvInstruction>? LoadAddress(ITranspilerValueContext context);
        IEnumerable<SpirvInstruction>? Load(ITranspilerValueContext context);
        IEnumerable<SpirvInstruction>? Store(ITranspilerValueContext context, ValueStackEntry value);
        IEnumerable<SpirvInstruction> LoadIndirect(ITranspilerValueContext context);
        IEnumerable<SpirvInstruction> StoreIndirect(ITranspilerValueContext context, StackEntry entry);
    }

    internal class BaseValueBehaviour : IValueBehaviour
    {
        public virtual IEnumerable<SpirvInstruction>? LoadAddress(ITranspilerValueContext context) { return null; }
        public virtual IEnumerable<SpirvInstruction>? Load(ITranspilerValueContext context) { return null; }
        public virtual IEnumerable<SpirvInstruction>? Store(ITranspilerValueContext context, ValueStackEntry value) { return null; }

        public virtual IEnumerable<SpirvInstruction> LoadIndirect(ITranspilerValueContext context)
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

        public virtual IEnumerable<SpirvInstruction> StoreIndirect(ITranspilerValueContext context, StackEntry entry)
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
