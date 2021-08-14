// This file was generated. Do not modify.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace cilspirv.Spirv.Ops
{
    public sealed record OpFunctionCall : FunctionInstruction
    {
        public ID ResultType { get; init; }
        public ID Result { get; init; }
        public ID Function { get; init; }
        public ImmutableArray<ID> Arguments { get; init; }

        public override OpCode OpCode => OpCode.OpFunctionCall;
        public override int WordCount => 1 + 1 + 1 + 1 + Arguments.Length;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                var result = Enumerable.Empty<ID>();
                result = result.Concat(Arguments);
                return result;
            }
        }

        public OpFunctionCall() {}

        private OpFunctionCall(IReadOnlyList<uint> codes, Range range)
        {
            var (start, end) = range.GetOffsetAndLength(codes.Count);
            end += start;
            var i = start;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Function = new ID(codes[i++]);
            Arguments = codes.Skip(i).Take(end - i)
                .Select(x => new ID(x))
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
            codes[i++] = mapID(Function);
            foreach (var x in Arguments)
            {
                codes[i++] = mapID(x);
            }
        }
    }
}

