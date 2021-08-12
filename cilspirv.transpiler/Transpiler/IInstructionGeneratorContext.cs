using System;
using System.Collections.Generic;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal interface IInstructionGeneratorContext
    {
        ID CreateID();
        ID IDOf(SpirvType type);
    }

    internal interface IInstructionGeneratable
    {
        IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context, out ID? mainResultId);
    }
}
