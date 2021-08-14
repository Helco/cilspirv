using System;
using System.Collections.Generic;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    // necessary so we can store and load an ID for it
    internal sealed record TranspilerExtInstructionSet : IInstructionGeneratable
    {
        public string Name { get; }

        public TranspilerExtInstructionSet(string name) => Name = name;

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context)
        {
            yield return new OpExtInstImport()
            {
                Result = context.CreateIDFor(this),
                Name = Name
            };
        }
    }
}
