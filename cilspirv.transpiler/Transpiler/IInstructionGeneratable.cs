using System;
using System.Collections.Generic;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal interface IInstructionGeneratable
    {
        IEnumerator<Instruction> GenerateInstructions(IIDMapper context);
    }

    internal interface IDebugInstructionGeneratable : IInstructionGeneratable
    {
        IEnumerator<Instruction> GenerateDebugInfo(IIDMapper context);
    }
}
