using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler.Values
{
    internal class GlobalVariableReference : IValueBehaviour
    {
        private readonly GlobalVariable variable;

        public GlobalVariableReference(GlobalVariable variable)
        {
            this.variable = variable;
        }

        IEnumerable<Instruction>? IValueBehaviour.Load(ITranspilerValueContext context) =>
            ((IValueBehaviour)variable).LoadAddress(context);

        IEnumerable<Instruction>? IValueBehaviour.Store(ITranspilerValueContext context, ValueStackEntry value) =>
            throw new InvalidOperationException("Cannot modify a global variable reference");

        IEnumerable<Instruction>? IValueBehaviour.LoadAddress(ITranspilerValueContext context) =>
            throw new InvalidOperationException("Cannot take the address of a global variable reference");

        IEnumerable<Instruction> IValueBehaviour.LoadIndirect(ITranspilerValueContext context) =>
            ((IValueBehaviour)this).Load(context)!;

        IEnumerable<Instruction> IValueBehaviour.StoreIndirect(ITranspilerValueContext context, StackEntry entry) =>
            ((IValueBehaviour)variable).Store(context, entry as ValueStackEntry ??
                throw new InvalidOperationException("StoreIndirect for global variable reference is not a value"))!;
    }
}
