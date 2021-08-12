using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace cilspirv.SourceGen
{
    partial class SourceGen
    {
        private static void GenerateInstructionClasses(CoreGrammar.Rootobject coreGrammar, string outputPath)
        {
            using var output = new StreamWriter(outputPath);
            output.WriteLines(Header());
            var classes = coreGrammar.instruction_printing_class.Skip(1);
            foreach (var @class in classes)
                output.WriteLines(InstructionClass(@class));
            output.WriteLines(Footer());

            static IEnumerable<string> Header()
            {
                yield return "// This file was generated. Do not modify.";
                yield return "";
                yield return "namespace cilspirv.Spirv";
                yield return "{";
            }

            static IEnumerable<string> InstructionClass(CoreGrammar.Instruction_Printing_Class @class)
            {
                yield return $"    /// <summary>{@class.heading}</summary>";
                yield return $"    public abstract record {MapInstructionClassName(@class.tag)} : Instruction {{}}";
            }

            static IEnumerable<string> Footer()
            {
                yield return "}";
                yield return "";
            }
        }
    }
}
