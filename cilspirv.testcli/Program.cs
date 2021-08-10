using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
namespace cilspirv.testcli
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = typeof(test_shaders.Simple).Assembly;
            var assemblyDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(assembly.Location);
            Console.WriteLine(assemblyDef.MainModule.Types[1].FullName);
        }
    }
}
