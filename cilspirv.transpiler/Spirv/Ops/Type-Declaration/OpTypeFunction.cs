// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpTypeFunction : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public ID ReturnType { get; init; }
        public ImmutableArray<ID> Parameters { get; init; }

        public override OpCode OpCode => OpCode.OpTypeFunction;
        public override int WordCount => 1 + 1 + 1 + Parameters.Length + ExtraWordCount;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { Result, ReturnType });
                result = result.Concat(Parameters);
                return result;
            }
        }

        public OpTypeFunction() {}

        private OpTypeFunction(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            ReturnType = new ID(codes[i++]);
            Parameters = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
                .ToImmutableArray();
            i = end;
            ExtraOperands = codes.Skip(i).Take(end - i)
                .Select(x => new ExtraOperand(x))
                .ToImmutableArray();
        }

        public override void Write(Span<uint> codes, Func<ID, uint> mapID)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = mapID(Result);
            codes[i++] = mapID(ReturnType);
            foreach (var x in Parameters)
            {
                codes[i++] = mapID(x);
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(ReturnType);
            foreach (var value in Parameters)
            {
                writer.Write(' ');
                writer.Write(value);
            }
            DisassembleExtras(writer);
        }
    }
}

