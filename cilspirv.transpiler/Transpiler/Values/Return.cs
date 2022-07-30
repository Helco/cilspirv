using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler.Values
{
    internal abstract class Return : ITranspilerValueBehaviour
    {
        public IEnumerable<Instruction> Load(ITranspilerValueContext context) =>
            throw new InvalidOperationException("Cannot load a return value");

        public IEnumerable<Instruction> LoadAddress(ITranspilerValueContext context) =>
            throw new InvalidOperationException("Cannot load a return value address");

        public abstract IEnumerable<Instruction> Store(ITranspilerValueContext context, ValueStackEntry value);
    }

    internal class ValueReturn : Return
    {
        public SpirvType SpirvType { get; }

        public ValueReturn(SpirvType spirvType)
        {
            SpirvType = spirvType;
        }

        public override IEnumerable<Instruction> Store(ITranspilerValueContext context, ValueStackEntry value)
        {
            yield return new OpReturnValue()
            {
                Value = value.ID
            };
        }
    }

    internal class VariableReturn : Return
    {
        public Variable Variable { get; }

        public VariableReturn(Variable variable)
        {
            Variable = variable;
        }

        public override IEnumerable<Instruction> Store(ITranspilerValueContext context, ValueStackEntry value)
        {
            var storeInstructions = ((ITranspilerValueBehaviour)Variable).Store(context, value);
            return storeInstructions!.Append(new OpReturn());
        }
    }
}
