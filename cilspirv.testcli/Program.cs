using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using cilspirv.Spirv;

namespace cilspirv.testcli
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = typeof(test_shaders.Simple).Assembly;
            var assemblyDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(assembly.Location);
            Console.WriteLine(assemblyDef.MainModule.Types[1].FullName);

            var module = new SpirvModule(new FileStream("semantics.spv", FileMode.Open));
            module.Write(new FileStream("semantics.new.spv", FileMode.Create));
        }
    }
}
