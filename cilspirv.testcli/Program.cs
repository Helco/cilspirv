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

            var moduleTypeDefs = assemblyDef.MainModule.Types.Where(
                t => t.GetCustomAttributes<ModuleAttributeBase>(exactType: false).Any());
            if (!moduleTypeDefs.Any())
                throw new InvalidDataException("Could not find a module class");

            foreach (var moduleTypeDef in moduleTypeDefs)
            {
                Console.WriteLine($"Transpiling {moduleTypeDef.Name}");
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
                    unit.MarkEntryPoint(entryPoint);
                }
                unit.TranspileBodies();
                unit.WriteSpirvModule(new FileStream($"{moduleTypeDef.Name}.spv", FileMode.Create));
            }
        }
    }
}
