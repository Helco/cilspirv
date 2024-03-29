﻿using System;
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
        [EntryPoint(ExecutionModel.Fragment)] public void RefVar([Output, Location(0)] ref int a) => a *= 2;

        public struct ValueStruct
        {
            public int m;
        }
        public int RealValueStruct(ValueStruct s) => s.m * 2;
        public void RefValueStruct(ref ValueStruct s) => s.m *= 2;

        public struct VarStruct
        {
            [Output] public int m;
        }

        public int VarStruct_(VarStruct s) => s.m * 2;
        public void RefVarStruct(ref VarStruct s) => s.m *= 2;

        public struct VarTemplateStruct
        {
            [Location(0)] public int m;
        }
        public int VarTemplateStruct_([Input] VarTemplateStruct s) => s.m * 2;
        public int RefVarTemplateStruct([Input] in VarTemplateStruct s) => s.m * 2;
    }
}

namespace cilspirv.transpiler.test.Values
{
    public class TestParameter : ApprovalTranspileFixture<Modules.Parameter>
    {
        [Test] public void Real() => VerifyNonEntryFunction(nameof(Modules.Parameter.Real));
        //[Test] public void Ref() => VerifyFunction(nameof(Modules.Parameter.Ref)); // TODO: support by-reference function parameters
        [Test] public void Var() => VerifyNonEntryFunction(nameof(Modules.Parameter.Var));
        [Test] public void RefVar() => VerifyEntryPoint(nameof(Modules.Parameter.RefVar));
        //[Test] public void RealValueStruct() => VerifyFunction(nameof(Modules.Parameter.RealValueStruct)); // value structs cannot be supported directly as SPIR-V parameters have no address
        
        //[Test] public void RefValueStruct() => VerifyNonEntryFunction(nameof(Modules.Parameter.RefValueStruct));
        [Test] public void VarStruct_() => VerifyNonEntryFunction(nameof(Modules.Parameter.VarStruct_));
        [Test] public void RefVarStruct() => VerifyNonEntryFunction(nameof(Modules.Parameter.RefVarStruct));
        [Test] public void VarTemplateStruct() => VerifyNonEntryFunction(nameof(Modules.Parameter.VarTemplateStruct_));
        [Test] public void RefVarTemplateStruct() => VerifyNonEntryFunction(nameof(Modules.Parameter.RefVarTemplateStruct));
    }
}
