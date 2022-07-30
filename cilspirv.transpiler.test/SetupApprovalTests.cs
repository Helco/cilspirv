using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ApprovalTests.Reporters;
using DiffEngine;
using NUnit.Framework;

[assembly: UseReporter(typeof(DiffReporter))]

namespace cilspirv.transpiler.test
{
    internal class SetupApprovalTests
    {
        [ModuleInitializer]
        internal static void Setup()
        {
            DiffTools.UseOrder(DiffTool.VisualStudio);
        }

        [Test]
        public void IsSpirvValPresent()
        {
            var process = new Process()
            {
                StartInfo =
                {
                    FileName = "spirv-val",
                    Arguments = "--version",
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            Assert.AreEqual(0, process.ExitCode);
            Assert.True(output.Contains("SPIR-V 1.0"));
        }
    }
}
