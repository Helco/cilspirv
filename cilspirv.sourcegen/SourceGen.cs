using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;

namespace cilspirv.SourceGen
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var generateSpirvEnums =
                context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.GenerateSpirvEnums", out var generateSpirvEnumSwitch) &&
                generateSpirvEnumSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

            var generateSpirvInstructions =
                context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.GenerateSpirvInstructions", out var generateSpirvInstructionsSwitch) &&
                generateSpirvInstructionsSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

            var generateSpirvExtInstructions =
                context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.GenerateSpirvExtInstructions", out var generateSpirvExtInstructionsSwitch) &&
                generateSpirvExtInstructionsSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

            var assembly = typeof(SourceGenerator).Assembly;
            var coreGrammarStream = assembly.GetManifestResourceStream("spirv.core.grammar.json");
            var coreGrammarReader = new StreamReader(coreGrammarStream);
            using var coreGrammarJsonReader = new JsonTextReader(coreGrammarReader);
            var coreGrammar = new JsonSerializer().Deserialize<CoreGrammar.Rootobject>(coreGrammarJsonReader);
        }

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
