using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using cilspirv.Spirv;
using Mono.Cecil;
using NUnit.Framework;

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
            transpiler.Module.Capabilities.Add(Capability.Shader);
            transpiler.Module.Capabilities.Add(Capability.Linkage); // there might be no entry point
            if (isEntryPoint)
                transpiler.MarkEntryPoint(method);
            else
                transpiler.MarkNonEntryFunction(method);
            transpiler.TranspileBodies();

            using var memoryStream = new MemoryStream();
            transpiler.WriteSpirvModule(memoryStream, leaveOpen: true);
            memoryStream.Position = 0;

            var reparsed = new SpirvModule(memoryStream, leaveOpen: true);
            ApprovalTests.Approvals.Verify(reparsed.Disassemble());

            memoryStream.Position = 0;
            var process = new Process()
            {
                StartInfo =
                {
                    FileName = "spirv-val",
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            memoryStream.WriteTo(process.StandardInput.BaseStream);
            process.StandardInput.Close();
            process.WaitForExit();

            var output = process.StandardError.ReadToEnd();
            Assert.AreEqual("", output, output);
            Assert.AreEqual(0, process.ExitCode);
        }

        protected void VerifyEntryPoint(string functionName) => VerifyFunction(functionName, true);
        protected void VerifyNonEntryFunction(string functionName) => VerifyFunction(functionName, false);
    }
}
