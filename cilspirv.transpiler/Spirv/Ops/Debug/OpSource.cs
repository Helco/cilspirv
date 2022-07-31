// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpSource : DebugInstruction
    {
        public SourceLanguage SourceLanguage { get; init; }
        public LiteralNumber Version { get; init; }
        public ID? File { get; init; }
        public LiteralString? Source { get; init; }

        public override OpCode OpCode => OpCode.OpSource;
        public override int WordCount => 1 + 1 + 1 + (File.HasValue ? 1 : 0) + (Source.HasValue ? Source.Value.WordCount : 0) + ExtraWordCount;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { File }
                    .Where(o => o.HasValue)
                    .Select(o => o!.Value));
                return result;
            }
        }

        public OpSource() {}

        private OpSource(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            SourceLanguage = (SourceLanguage)codes[i++];
            Version = (LiteralNumber)codes[i++];
            if (i < end)
                File = new ID(codes[i++]);
            if (i < end)
                Source = new LiteralString(codes, ref i);
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
            codes[i++] = (uint)SourceLanguage;
            codes[i++] = Version.Value;
            if (File.HasValue)
            {
                codes[i++] = mapID(File.Value);
            }
            if (Source.HasValue)
            {
                Source.Value.Write(codes, ref i);
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(SourceLanguage);
            writer.Write(' ');
            writer.Write(Version);
            writer.Write(' ');
            writer.Write(File);
            writer.Write(' ');
            writer.Write(Source);
            DisassembleExtras(writer);
        }
    }
}

