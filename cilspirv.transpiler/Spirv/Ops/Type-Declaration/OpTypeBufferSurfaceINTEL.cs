// This file was generated. Do not modify.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.VectorComputeINTEL })]
    public sealed record OpTypeBufferSurfaceINTEL : TypeDeclarationInstruction
    {
        public ID Result { get; init; }
        public AccessQualifier AccessQualifier { get; init; }

        public override OpCode OpCode => OpCode.OpTypeBufferSurfaceINTEL;
        public override int WordCount => 1 + 1 + 1 + ExtraWordCount;
        public override ID? ResultID => Result;

        public override IEnumerable<ID> AllIDs => new[] { Result }.Concat(ExtraIDs);

        public OpTypeBufferSurfaceINTEL() {}

        private OpTypeBufferSurfaceINTEL(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            Result = new ID(codes[i++]);
            AccessQualifier = (AccessQualifier)codes[i++];
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
            codes[i++] = (uint)AccessQualifier;
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }

        public override void Disassemble(TextWriter writer)
        {
            base.Disassemble(writer);
            writer.Write(' ');
            writer.Write(AccessQualifier);
        }
    }
}

