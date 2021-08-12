// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    [DependsOn(Version = "None", Capabilities = new[] { Capability.AtomicFloat16AddEXT, Capability.AtomicFloat32AddEXT, Capability.AtomicFloat64AddEXT }, Extensions = new[] { "SPV_EXT_shader_atomic_float_add" })]
    public sealed record OpAtomicFAddEXT : AtomicInstruction
    {
        public ID ResultType1 { get; init; }
        public ID Result2 { get; init; }
        public ID Pointer { get; init; }
        public ID Memory { get; init; }
        public ID Semantics { get; init; }
        public ID Value { get; init; }

        public override OpCode OpCode => OpCode.OpAtomicFAddEXT;
        public override int WordCount => 1 + 1 + 1 + 1 + 1 + 1 + 1;
        public override ID? ResultID => Result2;
        public override ID? ResultTypeID => ResultType1;

        public override IEnumerable<ID> AllIDs => new[] { ResultType1, Result2, Pointer, Memory, Semantics, Value };

        public OpAtomicFAddEXT() {}

        private OpAtomicFAddEXT(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType1 = new ID(codes[i++]);
            Result2 = new ID(codes[i++]);
            Pointer = new ID(codes[i++]);
            Memory = new ID(codes[i++]);
            Semantics = new ID(codes[i++]);
            Value = new ID(codes[i++]);
        }

        public override void Write(Span<uint> codes)
        {
            if (codes.Length < WordCount)
                throw new ArgumentException("Output span too small", nameof(codes));
            var i = 0;
            codes[i++] = InstructionCode;
            codes[i++] = ResultType1.Value;
            codes[i++] = Result2.Value;
            codes[i++] = Pointer.Value;
            codes[i++] = Memory.Value;
            codes[i++] = Semantics.Value;
            codes[i++] = Value.Value;
        }
    }
}

