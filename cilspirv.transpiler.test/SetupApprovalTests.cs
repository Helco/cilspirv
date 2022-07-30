using System;
using System.Runtime.CompilerServices;
using ApprovalTests.Reporters;
using DiffEngine;

[assembly: UseReporter(typeof(DiffReporter))]

namespace cilspirv.transpiler.test
{
    internal static class SetupApprovalTests
    {
        [ModuleInitializer]
        internal static void Setup()
        {
            DiffTools.UseOrder(DiffTool.VisualStudio);
        }
    }
}
