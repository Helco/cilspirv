using System;
using System.Collections.Generic;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler.Declarations
{
    // necessary so we can store and load an ID for it
    internal sealed record ExtInstructionSet : IInstructionGeneratable
    {
        public string Name { get; }

        public ExtInstructionSet(string name) => Name = name;

        public IEnumerator<Instruction> GenerateInstructions(IIDMapper context)
        {
            yield return new OpExtInstImport()
            {
                Result = context.CreateIDFor(this),
                Name = Name
            };
        }
    }
}
