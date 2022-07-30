using System;
using System.IO;
using System.Linq;
using cilspirv.Spirv;
using Mono.Cecil;

namespace cilspirv.transpiler.test
{
    public class ApprovalTranspileFixture
    {
        protected static readonly Lazy<AssemblyDefinition> assemblyLazy = new(
            () => AssemblyDefinition.ReadAssembly(typeof(ApprovalTranspileFixture<>).Assembly.Location),
            isThreadSafe: true);

        protected static ModuleDefinition ThisModule => assemblyLazy.Value.Modules[0];
    }

    public class ApprovalTranspileFixture<T> : ApprovalTranspileFixture
    {
        private TypeDefinition ThisModuleType => ThisModule.GetType(typeof(T).FullName);

        protected void VerifyFunction(string functionName, bool isEntryPoint)
        {
            var method = ThisModuleType.Methods.First(m => m.Name == functionName);

            var transpiler = new Transpiler.Transpiler(ThisModuleType);
            if (isEntryPoint)
                transpiler.MarkEntryPoint(method);
            else
                transpiler.MarkNonEntryFunction(method);
            transpiler.TranspileBodies();

            using var memoryStream = new MemoryStream();
            transpiler.WriteSpirvModule(memoryStream, leaveOpen: true);
            memoryStream.Position = 0;

            var reparsed = new SpirvModule(memoryStream);
            ApprovalTests.Approvals.Verify(reparsed.Disassemble());
        }

        protected void VerifyEntryPoint(string functionName) => VerifyFunction(functionName, true);
        protected void VerifyNonEntryFunction(string functionName) => VerifyFunction(functionName, false);
    }
}
