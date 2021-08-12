using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace cilspirv.Spirv
{
    public enum OpCode { }

    /// <summary>
    /// A SPIR-V Instruction
    /// </summary>
    public abstract record Instruction
    {
        /// <summary>
        /// If non-null, this the result ID of the op
        /// </summary>Var
        public virtual ID? ResultID => null;

        /// <summary>
        /// If non-null, this the result type ID of the op
        /// </summary>
        public virtual ID? ResultTypeID => null;

        public abstract int WordCount { get; }

        public abstract OpCode OpCode { get; }

        protected abstract void WriteCode(Span<uint> code);

        public virtual IEnumerable<ID> AllIDs => Array.Empty<ID>();

        /// <summary>
        /// Opcode: The 16 high-order bits are the WordCount of the
        /// instruction.The 16 low-order bits are the opcode enumerant.
        /// </summary>
        public uint InstructionCode => (uint)OpCode + (uint)(WordCount << 16);

        private record LayoutInfo
        {
            public LayoutInfo(ConstructorInfo defaultCtor, ConstructorInfo codeCtor)
            {
                DefaultCtor = defaultCtor;
                CodeCtor = codeCtor;
            }

            public ConstructorInfo DefaultCtor { get; }
            public ConstructorInfo CodeCtor { get; }
        }

        private static IReadOnlyDictionary<OpCode, LayoutInfo>? cachedOps;

        private static IReadOnlyDictionary<OpCode, LayoutInfo> FindInstructions()
        {
            var instructionTypes = typeof(Instruction)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Instruction)) && !t.IsAbstract)
                .Where(t => t != typeof(OpUnknown));

            var ops = new Dictionary<OpCode, LayoutInfo>();
            foreach (var type in instructionTypes)
            {
                var defaultCtor = type.GetConstructor(Array.Empty<Type>());
                var codeCtor = type.GetConstructor(BindingFlags.NonPublic, null, new[] { typeof(IReadOnlyList<uint>), typeof(Range) }, null);
                if (defaultCtor == null || codeCtor == null)
                    throw new InvalidProgramException($"Invalid constructors for {type.Name}");
                var obj = (Instruction)defaultCtor.Invoke(null);

                ops.Add(obj.OpCode, new LayoutInfo(defaultCtor, codeCtor));
            }
            return ops;
        }

        public static Instruction Read(IReadOnlyList<uint> codes)
        {
            int start = 0;
            return Read(codes, ref start);
        }

        public static Instruction Read(IReadOnlyList<uint> codes, ref int start)
        {
            if (cachedOps == null)
                cachedOps = FindInstructions();

            var icode = codes[start++];
            var opcode = (OpCode)(icode & 0x0000FFFF);
            var wordCount = icode >> 16;
            if (start + wordCount > codes.Count)
                throw new FormatException("End of codes");

            var op = cachedOps.TryGetValue(opcode, out var info)
                ? (Instruction)info.CodeCtor.Invoke(new object[] { codes, start..(start + (int)wordCount) })
                : new OpUnknown(opcode, codes.Skip(start).Take((int)wordCount));
            start += (int)wordCount;
            return op;
        }

        public void Write(Span<uint> code)
        {
            if (code.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(code));
            code[0] = InstructionCode;
            WriteCode(code.Slice(1));
        }

        protected static string StrOf(ID id) => id.ToString();
        protected static string StrOf(LiteralNumber nr) => nr.ToString();
        protected static string StrOf(LiteralString str) => str.ToString();
        protected static string StrOf(ID? id) => string.Format("<{0}>", id?.ToString() ?? "null");
        protected static string StrOf(ID[] ids) => string.Format("[{0}]", ids == null ? "null" : ids.Length == 0 ? " " : ids.Select(i => i.ToString()).Aggregate((s1, s2) => s1 + ", " + s2));
        protected static string StrOf(LiteralNumber[] nrs) => string.Format("[{0}]", nrs == null ? "null" : nrs.Length == 0 ? " " : nrs.Select(i => i.ToString()).Aggregate((s1, s2) => s1 + ", " + s2));
        protected static string StrOf<T>(T e) where T : struct => e.ToString() ?? "null";
        protected static string StrOf<T>(T[] es) => string.Format("[{0}]", es == null ? "null" : es.Length == 0 ? " " : es.Select(i => i?.ToString() ?? "null").Aggregate((s1, s2) => s1 + ", " + s2));
        protected static string StrOf<U, V>((U, V)[] p) => string.Format("[{0}]", p == null ? "null" : p.Length == 0 ? " " : p.Select(i => i.ToString()).Aggregate((s1, s2) => s1 + ", " + s2));
    }
}
