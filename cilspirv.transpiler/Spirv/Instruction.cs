﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using cilspirv.Spirv.Ops;

namespace cilspirv.Spirv
{
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

        public ImmutableArray<ExtraOperand> ExtraOperands { get; init; }

        public abstract int WordCount { get; }
        protected int ExtraWordCount => ExtraOperands.IsDefaultOrEmpty
            ? 0
            : ExtraOperands.Sum(o => o.WordCount);

        public abstract OpCode OpCode { get; }

        public abstract void Write(Span<uint> code, Func<ID, uint> mapID);

        public virtual IEnumerable<ID> AllIDs => Array.Empty<ID>();
        protected IEnumerable<ID> ExtraIDs => ExtraOperands.IsDefaultOrEmpty
            ? Enumerable.Empty<ID>()
            : ExtraOperands
                .Where(o => o.Kind == ExtraOperandKind.ID)
                .Select(o => o.ID);

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
                var codeCtor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(IReadOnlyList<uint>), typeof(Range) }, null);
                if (defaultCtor == null || codeCtor == null)
                    throw new InvalidProgramException($"Invalid constructors for {type.Name}");
                var obj = (Instruction)defaultCtor.Invoke(null);

                if (ops.TryGetValue(obj.OpCode, out var prevLayout))
                {
                    if (type.Name.Length < prevLayout.DefaultCtor.DeclaringType?.Name?.Length)
                        ops[obj.OpCode] = new LayoutInfo(defaultCtor, codeCtor);
                }
                else
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
            if (start + wordCount - 1 > codes.Count)
                throw new FormatException("End of codes");

            var op = cachedOps.TryGetValue(opcode, out var info)
                ? (Instruction)info.CodeCtor.Invoke(new object[] { codes, start..(start + (int)wordCount - 1) })
                : new OpUnknown(opcode, codes.Skip(start).Take((int)wordCount));
            start += (int)wordCount;
            return op;
        }

        protected ImmutableArray<LiteralNumber> ReadContextDependentNumber(IReadOnlyList<uint> codes, ref int i, int end)
        {
            var result = codes.Skip(i).Take(end - i).Select(n => (LiteralNumber)n).ToImmutableArray();
            i = end;
            return result;
        }

        public virtual void Disassemble(TextWriter writer)
        {
            if (ResultID.HasValue)
            {
                writer.Write(ResultID);
                writer.Write(" = ");
            }
            writer.Write(OpCode);
        }

        protected void DisassembleExtras(TextWriter writer)
        {
            foreach (var operand in ExtraOperands)
            {
                writer.Write(' ');
                writer.Write(operand);
            }
        }

        protected void DisassembleExtraNumbers(TextWriter writer)
        {
            foreach (var operand in ExtraOperands.SelectMany(o => o.ToWords()))
            {
                writer.Write(' ');
                writer.Write(operand);
            }
        }

        protected void DisassembleExtraIDs(TextWriter writer)
        {
            var extraIds = ExtraOperands
                .SelectMany(o => o.ToWords())
                .Select(r => new ID(r));
            foreach (var operand in extraIds)
            {
                writer.Write(' ');
                writer.Write(operand);
            }
        }

        protected void DisassembleExtraStrings(TextWriter writer)
        {
            var extraStrings = ExtraOperands
                .SelectMany(o => o.ToWords())
                .SegmentsByLast(w => w < 0xff_ff_ff)
                .Select(words => new LiteralString(words.ToArray()));
            foreach (var operand in extraStrings)
            {
                writer.Write(' ');
                writer.Write(operand);
            }
        }
    }
}
