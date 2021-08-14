// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Capabilities = new[] { Capability.Addresses, Capability.VariablePointers, Capability.VariablePointersStorageBuffer, Capability.PhysicalStorageBufferAddresses })]
    public sealed record OpPtrAccessChain : MemoryInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Base { get; init; }
        public ID Element { get; init; }
        public ImmutableArray<ID> Indexes { get; init; }

        public override OpCode OpCode => OpCode.OpPtrAccessChain;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + Indexes.Length + ExtraWordCount;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = ExtraIDs;
                result = result.Concat(new[] { ResultType, Result, Base, Element });
                result = result.Concat(Indexes);
                return result;
            }
        }

        public OpPtrAccessChain() {}

        private OpPtrAccessChain(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Base = new ID(codes[i++]);
            Element = new ID(codes[i++]);
            Indexes = codes.Skip(i).Take(end - i)
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
            codes[i++] = mapID(ResultType);
            codes[i++] = mapID(Result);
            codes[i++] = mapID(Base);
            codes[i++] = mapID(Element);
            foreach (var x in Indexes)
            {
                codes[i++] = mapID(x);
            }
            if (!ExtraOperands.IsDefaultOrEmpty)
                foreach (var o in ExtraOperands)
                    o.Write(codes, ref i, mapID);
        }
    }
}

