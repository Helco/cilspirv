// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "1.4", Extensions = new[] { "SPV_GOOGLE_decorate_string", "SPV_GOOGLE_hlsl_functionality1" })]
    public sealed record OpMemberDecorateStringGOOGLE : AnnotationInstruction
    {
        public ID StructType { get; init; }
        public LiteralNumber Member { get; init; }
        public Decoration Decoration { get; init; }

        public override OpCode OpCode => OpCode.OpMemberDecorateStringGOOGLE;
        public override int WordCount => 1 + 1 + 1 + 1 + ExtraWordCount;

        public override IEnumerable<ID> AllIDs => new[] { StructType }.Concat(ExtraIDs);

        public OpMemberDecorateStringGOOGLE() {}

        private OpMemberDecorateStringGOOGLE(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            StructType = new ID(codes[i++]);
            Member = (LiteralNumber)codes[i++];
            Decoration = (Decoration)codes[i++];
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
            codes[i++] = mapID(StructType);
            codes[i++] = Member.Value;
            codes[i++] = (uint)Decoration;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(StructType);
            writer.Write(' ');
            writer.Write(Member);
            writer.Write(' ');
            writer.Write(Decoration);
            DisassembleExtraStrings(writer);
        }
    }
}

