﻿using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

namespace cilspirv.Transpiler
{
    internal class TranspilerBlock : IInstructionGeneratable
    {
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public IEnumerator<Instruction> GenerateInstructions(IInstructionGeneratorContext context) =>
            Instructions.Prepend(new OpLabel()
            {
                Result = context.CreateIDFor(this)
            }).GetEnumerator();
    }
}