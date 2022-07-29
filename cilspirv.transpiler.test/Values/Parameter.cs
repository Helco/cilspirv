using System;
using NUnit.Framework;
using cilspirv.Library;
using cilspirv.Spirv;
using System.Linq;
using System.IO;

namespace cilspirv.transpiler.test.Values.Modules
{
    public class Parameter
    {
        public int Real(int a) => a * 2;
        public void Ref(ref int a) => a = 45;
        public int Var([Input] int a) => a * 2;
        public void RefVar([Input] ref int a) => a *= 2;

        public struct ValueStruct
        {
            public int m;
        }
        public int RealValueStruct(ValueStruct s) => s.m * 2;
        public void RefValueStruct(ref ValueStruct s) => s.m *= 2;

        public struct VarStruct
        {
            [Input] public int m;
        }

        public int VarStruct_(VarStruct s) => s.m * 2;
        public void RefVarStruct(ref VarStruct s) => s.m *= 2;
    }
}

namespace cilspirv.transpiler.test.Values
{
    public class TestParameter
    {
        // we cannot use TestCaseSource or similar as ApprovalTests does not support this

        private void VerifyFunction(string functionName)
        {
            var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(typeof(TestParameter).Assembly.Location);
            var moduleType = assembly.Modules[0].GetType(typeof(Modules.Parameter).FullName);
            var method = moduleType.Methods.First(m => m.Name == functionName);
            var transpiler = new Transpiler.Transpiler(moduleType);
            transpiler.MarkNonEntryFunction(method);
            transpiler.TranspileBodies();
            using var memoryStream = new MemoryStream();
            transpiler.WriteSpirvModule(memoryStream, leaveOpen: true);
            memoryStream.Position = 0;
            var reparsed = new SpirvModule(memoryStream);
            ApprovalTests.Approvals.Verify(reparsed.Disassemble());
        }

        [Test] public void Real() => VerifyFunction(nameof(Modules.Parameter.Real));
        //[Test] public void Ref() => VerifyFunction(nameof(Modules.Parameter.Ref)); // TODO: support by-reference function parameters
        [Test] public void Var() => VerifyFunction(nameof(Modules.Parameter.Var));
        //[Test] public void RefVar() => VerifyFunction(nameof(Modules.Parameter.RefVar));
        //[Test] public void RealValueStruct() => VerifyFunction(nameof(Modules.Parameter.RealValueStruct)); // value structs cannot be supported directly as SPIR-V parameters have no address
        //[Test] public void RefValueStruct() => VerifyFunction(nameof(Modules.Parameter.RefValueStruct));
        [Test] public void VarStruct_() => VerifyFunction(nameof(Modules.Parameter.VarStruct_));
        [Test] public void RefVarStruct() => VerifyFunction(nameof(Modules.Parameter.RefVarStruct));
    }
}
