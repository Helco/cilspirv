using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using cilspirv.Library;
using cilspirv.Spirv;
using cilspirv.Transpiler;

namespace cilspirv.testcli
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = typeof(test_shaders.Simple).Assembly;
            var assemblyDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(assembly.Location, new Mono.Cecil.ReaderParameters()
            {
                ReadSymbols = true
            });

            var moduleTypeDef = assemblyDef.MainModule.Types.FirstOrDefault(
                t => t.GetCustomAttributes<ModuleAttributeBase>(exactType: false).Any());
            if (moduleTypeDef == null)
                throw new InvalidDataException("Could not find a module class");

            var entryPoints = moduleTypeDef.Methods.Where(
                method => method.GetCustomAttributes<EntryPointAttribute>().Any())
                .ToArray();
            if (!entryPoints.Any())
                throw new InvalidDataException($"Module class {moduleTypeDef.FullName} has no entry point");

            var unit = new Transpiler.Transpiler(moduleTypeDef);
            unit.Library.Mappers.Add(new SystemNumericsMapper());
            unit.ExtractModuleAttributes();
            foreach (var entryPoint in entryPoints)
            {
                unit.TranspileEntryPoint(entryPoint);
            }
            unit.TranspileAllMethodBodies();
            unit.WriteSpirvModule(new FileStream($"{moduleTypeDef.Name}.spv", FileMode.Create));
        }
    }
}
