using System;
using NUnit.Framework;
using cilspirv.Library;
using cilspirv.Spirv;
using System.Linq;
using System.IO;

namespace cilspirv.transpiler.test.IDMapping.Modules
{
    public class TypeIDs
    {
        [EntryPoint(ExecutionModel.Fragment)]
        [return: Output, Location(0)]
        public int ImplicitBlockStructure([Uniform, Binding(0, 0)] int a, [Uniform, Binding(0, 0)] int b) => a + b;
    }
}

namespace cilspirv.transpiler.test.IDMapping
{
    public class TestIDs : ApprovalTranspileFixture<Modules.TypeIDs>
    {
        // Intended for a suspicion that the same structs are generated multiple times
        // But as the OpMemberDecorate decoration changes this is correct
        // As an optimizer would remove decorations it could then more easily check for
        // duplicated types as well, so it probably would not be worth adding deduplication
        // behaviour in the case debug information is not generated
        [Test] public void ImplicitBlockStructure() => VerifyEntryPoint(nameof(Modules.TypeIDs.ImplicitBlockStructure));
    }
}
