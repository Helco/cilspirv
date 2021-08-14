using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace cilspirv.SourceGen
{
    partial class SourceGen
    {
        private static void GenerateDecorations(CoreGrammar.Rootobject coreGrammar, string outputPath)
        {
            using var output = new StreamWriter(outputPath);
            output.WriteLines(Header());
            var decorationEnum = coreGrammar.operand_kinds.Single(e => e.kind == "Decoration");
            foreach (var decoration in decorationEnum.enumerants)
                output.WriteLines(Decoration(decoration));
            output.WriteLines(Footer());
        }

        static IEnumerable<string> Header()
        {
            yield return "// This file was generated. Do not modify.";
            yield return "using System;";
            yield return "";
            yield return "namespace cilspirv.Spirv";
            yield return "{";
            yield return "    public static class Decorations";
            yield return "    {";
        }

        static IEnumerable<string> Decoration(CoreGrammar.Enumerant decoration)
        {
            var obsolete = Obsolete(decoration.lastVersion);
            if (obsolete != null)
                yield return "        " + obsolete;
            yield return $"        public static DecorationEntry {decoration.enumerant}() => new DecorationEntry(Decoration.{decoration.enumerant});";
        }

        static IEnumerable<string> Footer()
        {
            yield return "    }";
            yield return "}";
            yield return "";
        }
    }
}
