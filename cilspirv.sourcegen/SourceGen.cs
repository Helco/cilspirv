using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace cilspirv.SourceGen
{
    internal static partial class SourceGen
    {
        public static void Main(string[] args)
        {
            var assembly = typeof(SourceGen).Assembly;
            var coreGrammarStream = assembly.GetManifestResourceStream("spirv.core.grammar.json");
            using var coreGrammarReader = new StreamReader(coreGrammarStream);
            var coreGrammar = JsonSerializer.Deserialize<CoreGrammar.Rootobject>(coreGrammarReader.ReadToEnd());

            GenerateEnums(coreGrammar, "SpirvEnums.cs");
            GenerateDecorations(coreGrammar, "Decorations.cs");
            GenerateInstructionClasses(coreGrammar, "InstructionClasses.cs");
            GenerateInstructionEnum(coreGrammar, "OpCode.cs");
            GenerateInstructions(coreGrammar, "Ops");
            GenerateImageClasses("Image.cs");
        }

        private static void WriteLines(this StreamWriter writer, IEnumerable<string> lines)
        {
            foreach (var line in lines)
                writer.WriteLine(line);
        }

        private static string MapInstructionClassName(string tag) => tag
            .Replace("_and_", "And")
            .Replace("-", "")
            .Replace("_", "") + "Instruction";

        private static string? Obsolete(string? lastVersion) =>
            string.IsNullOrWhiteSpace(lastVersion)
            ? null
            : $"[Obsolete(\"Last version for this enumerant was {lastVersion}\")]";

        private static string? DependsOn(string? version, string[]? capabilities, string[]? extensions)
        {
            version = string.IsNullOrWhiteSpace(version) ? null : version;
            capabilities ??= Array.Empty<string>();
            extensions ??= Array.Empty<string>();
            if (version == null && !capabilities.Any() && !extensions.Any())
                return null;

            var args = "";
            if (version != null)
                args += $"Version = \"{version}\"";
            if (capabilities.Any())
            {
                if (args.Any())
                    args += ", ";
                var caps = string.Join(", ", capabilities.Select(c => "Capability." + c));
                args += $"Capabilities = new[] {{ {caps} }}";
            }
            if (extensions.Any())
            {
                if (args.Any())
                    args += ", ";
                var exts = string.Join(", ", extensions.Select(e => $"\"{e}\""));
                args += $"Extensions = new[] {{ {exts} }}";
            }

            return $"[DependsOn({args})]";
        }
    }
}
