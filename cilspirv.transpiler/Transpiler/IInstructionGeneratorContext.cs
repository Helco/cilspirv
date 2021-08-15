using System;
using System.Collections.Generic;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{

    internal interface IInstructionGeneratorContext
    {
        TranspilerOptions Options { get; }

        ID CreateID();
        ID CreateIDFor(IInstructionGeneratable generatable);
        ID IDOf(IInstructionGeneratable generatable);
        IEnumerable<T> OfType<T>() where T : IInstructionGeneratable;
    }

    internal interface IInstructionGeneratable
    {
        IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context);
    }

    internal interface IWrapperInstructionGeneratable : IInstructionGeneratable
    {
        IInstructionGeneratable WrappedGeneratable { get; }
    }

    internal interface IDebugInstructionGeneratable : IInstructionGeneratable
    {
        IEnumerator<Instruction> GenerateDebugInfo(IInstructionGeneratorContext context);
    }
}
