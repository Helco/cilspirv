using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace cilspirv.SourceGen
{
    partial class SourceGen
    {
        private static void GenerateEnums(CoreGrammar.Rootobject coreGrammar, string outputPath)
        {
            using var output = new StreamWriter(outputPath);
            output.WriteLines(Header());
            var enums = coreGrammar.operand_kinds.Where(o => o.category == "BitEnum" || o.category == "ValueEnum");
            foreach (var @enum in enums)
                output.WriteLines(GenerateEnum(@enum));
            output.WriteLines(Footer());

            static IEnumerable<string> Header()
            {
                yield return "// This file was generated. Do not modify.";
                yield return "using System;";
                yield return "";
                yield return "namespace cilspirv.Spirv";
                yield return "{";
            }

            static IEnumerable<string> GenerateEnum(CoreGrammar.Operand_Kinds bitEnum)
            {
                if (!string.IsNullOrWhiteSpace(bitEnum.doc))
                    yield return $"    /// <summary>{bitEnum.doc}</sumary>";
                if (bitEnum.category == "BitEnum")
                    yield return "    [Flags]";
                yield return $"    public enum {bitEnum.kind} : uint";
                yield return  "    {";
                foreach (var enumerant in bitEnum.enumerants)
                {
                    var obsolete = Obsolete(enumerant.lastVersion);
                    if (obsolete != null)
                        yield return "        " + obsolete;
                    var dependsOn = EnumerantDependsOn(enumerant);
                    if (dependsOn != null)
                        yield return dependsOn;

                    var name = enumerant.enumerant;
                    if (name.StartsWith('1') || name.StartsWith('2') || name.StartsWith('3'))
                        name = bitEnum.kind + name;
                    yield return $"        {name} = {enumerant.value}U,";
                }
                yield return "    }";
            }

            static string? EnumerantDependsOn(CoreGrammar.Enumerant enumerant) =>
                "        " + DependsOn(enumerant.version, enumerant.capabilities, enumerant.extensions);

            static IEnumerable<string> Footer()
            {
                yield return "}";
                yield return "";
            }
        }
    }
}
