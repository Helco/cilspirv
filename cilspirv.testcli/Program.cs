using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using cilspirv.Spirv;
using cilspirv.Transpiler;

namespace cilspirv.testcli
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = typeof(test_shaders.Simple).Assembly;
            var assemblyDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(assembly.Location);
            Console.WriteLine(assemblyDef.MainModule.Types[1].FullName);

            var module = new TranspilerModule();
            module.Capabilities.Add(Capability.Shader);
            module.ExtInstructionSets.Add(new TranspilerExtInstructionSet("GLSL.std.450"));
            module.MemoryModel = MemoryModel.GLSL450;
            module.AddressingModel = AddressingModel.Logical;

            var voidType = module.GetTranspilerTypeFor(new SpirvVoidType());
            var mainFunction = new TranspilerFunction("main", voidType);
            mainFunction.Decorations.Add(Decorations.UserSemantic("SOMETHING"));
            module.Functions.Add(mainFunction);

            var context = new InstructionGenerator();
            var spirvModule = new SpirvModule()
            {
                Instructions = module.GenerateInstructions(context).ToArray(),
                Bound = context.Bound
            };
            spirvModule.Write(new FileStream("test.spv", FileMode.Create), mapID: context.MapFromTemporaryID);
        }
    }
}
