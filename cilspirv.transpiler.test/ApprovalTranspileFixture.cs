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

        protected void VerifyModule(params (string functionName, bool isEntryPoint)[] functions) =>
            VerifyModule(new Transpiler.TranspilerOptions(), functions);

        protected void VerifyModule(Transpiler.TranspilerOptions? options, params (string functionName, bool isEntryPoint)[] functions)
        {
            var hasEntryPoint = functions.Any(f => f.isEntryPoint);

            var transpiler = new Transpiler.Transpiler(ThisModuleType) { Options = options ?? new() };
            transpiler.Module.Capabilities.Add(Capability.Shader);
            if (!hasEntryPoint)
                transpiler.Module.Capabilities.Add(Capability.Linkage);
            foreach (var (functionName, isEntryPoint) in functions)
            {
                var method = ThisModuleType.Methods.First(m => m.Name == functionName);
                if (isEntryPoint)
                    transpiler.MarkEntryPoint(method);
                else
                    transpiler.MarkNonEntryFunction(method);
            }
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
                    Arguments = hasEntryPoint ? "--target-env vulkan1.2" : "",
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

        protected void VerifyEntryPoint(string functionName, Transpiler.TranspilerOptions? options = null) =>
            VerifyModule(options, (functionName, true));

        protected void VerifyNonEntryFunction(string functionName, Transpiler.TranspilerOptions? options = null) =>
            VerifyModule(options, (functionName, false));
    }
}
