using System.Runtime.InteropServices;
using NUnit.Framework;

namespace cilspirv.transpiler.test.Library.Modules
{
    public class StructOffsetScanner
    {
        public struct Unmarked
        {
            public int a, b;
        }
        public int Unmarked_(Unmarked m) => m.a;

        [StructLayout(LayoutKind.Sequential)]
        public struct Sequential
        {
            public int a, b;
        }
        public int Sequential_(Sequential s) => s.a;

        [StructLayout(LayoutKind.Explicit)]
        public struct Explicit
        {
            [FieldOffset(24)] public int a;
            [FieldOffset(16)] public int b;
        }
        public int Explicit_(Explicit s) => s.a;
    }
}

namespace cilspirv.transpiler.test.Library
{
    public class TestStructOffsetScanner : ApprovalTranspileFixture<Modules.StructOffsetScanner>
    {
        [Test] public void Unmarked() => VerifyNonEntryFunction(nameof(Modules.StructOffsetScanner.Unmarked_));
        [Test] public void Sequential() => VerifyNonEntryFunction(nameof(Modules.StructOffsetScanner.Sequential_));
        [Test] public void Explicit() => VerifyNonEntryFunction(nameof(Modules.StructOffsetScanner.Explicit_));
    }
}
