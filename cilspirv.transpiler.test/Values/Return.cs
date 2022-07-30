using System;
using NUnit.Framework;
using cilspirv.Library;
using cilspirv.Spirv;
using System.Linq;
using System.IO;

namespace cilspirv.transpiler.test.Values.Modules
{
    public class Return
    {
        public int Real() => 24;

        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output]
        public int Var() => 32;

        public struct ValueStruct
        {
            public int m;
        }
        public ValueStruct ValueStruct_(ValueStruct m) => m;

        public struct VarStruct
        {
            [Output] public int m;
        }
        [EntryPoint(ExecutionModel.Fragment)]
        public VarStruct VarStruct_(VarStruct s) => s;

        public void NonEntryVoid() { }
        [EntryPoint(ExecutionModel.Fragment)]
        public void EntryVoid() { }
    }
}

namespace cilspirv.transpiler.test.Values
{
    public class TestReturn : ApprovalTranspileFixture<Modules.Return>
    {
        [Test] public void Real() => VerifyNonEntryFunction(nameof(Modules.Return.Real));
        [Test] public void Var() => VerifyEntryPoint(nameof(Modules.Return.Var));
        [Test] public void ValueStruct_() => VerifyNonEntryFunction(nameof(Modules.Return.ValueStruct_));
        [Test] public void VarStruct_() => VerifyEntryPoint(nameof(Modules.Return.VarStruct_));
        [Test] public void NonEntryVoid() => VerifyNonEntryFunction(nameof(Modules.Return.NonEntryVoid));
        [Test] public void EntryVoid() => VerifyEntryPoint(nameof(Modules.Return.EntryVoid));
    }
}
