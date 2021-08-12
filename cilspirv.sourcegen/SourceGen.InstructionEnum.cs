using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace cilspirv.SourceGen
{
    partial class SourceGen
    {
        private static void GenerateInstructionEnum(CoreGrammar.Rootobject coreGrammar, string outputPath)
        {
            using var output = new StreamWriter(outputPath);
            output.WriteLines(Header());
            foreach (var instruction in coreGrammar.instructions.Where(i => i.@class != "@exclude"))
                output.WriteLines(Instruction(instruction));
            output.WriteLines(Footer());

            static IEnumerable<string> Header()
            {
                yield return "// This file was generated. Do not modify.";
                yield return "using System;";
                yield return "";
                yield return "namespace cilspirv.Spirv";
                yield return "{";
                yield return "    public enum OpCode : uint";
                yield return "    {";
            }

            static IEnumerable<string> Instruction(CoreGrammar.Instruction instruction)
            {
                var obsolete = Obsolete(instruction.lastVersion);
                if (obsolete != null)
                    yield return "        " + obsolete;

                var dependsOn = InstructionDependsOn(instruction);
                if (dependsOn != null)
                    yield return "        " + dependsOn;

                yield return $"        {instruction.opname} = {instruction.opcode}U,";
            }

            static IEnumerable<string> Footer()
            {
                yield return "    }";
                yield return "}";
                yield return "";
            }
        }

        private static string InstructionDependsOn(CoreGrammar.Instruction i) =>
            DependsOn(i.version, i.capabilities, i.extensions);
    }
}
