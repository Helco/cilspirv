using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpirvNet.Spirv.Enums;

// This file is auto-generated and should not be modified manually.

namespace SpirvNet.Spirv.Ops.Composite
{
    /// <summary>
    /// OpVectorInsertDynamic
    /// 
    /// Write a single, variably selected, component into a vector. 
    /// 
    /// Vector must be a vector type and is the vector that the non-written components will be taken from.
    /// 
    /// Index must be a scalar-integer 0-based index of which component to read.
    /// 
    /// What memory is written is undefined if Index&#8217;s value is less than zero or greater than or equal to the number of components in Vector.
    /// 
    /// The Result Type must be the same type as the type of Vector.
    /// </summary>
    public sealed class OpVectorInsertDynamic : CompositeInstruction
    {
        public override bool IsComposite => true;
        public override OpCode OpCode => OpCode.VectorInsertDynamic;
        public override ID? ResultID => Result;
        public override ID? ResultTypeID => ResultType;

        public ID ResultType;
        public ID Result;
        public ID Vector;
        public ID Component;
        public ID Index;

        #region Code
        public override string ToString() => "(" + OpCode + "(" + (int)OpCode + ")" + ", " + StrOf(ResultType) + ", " + StrOf(Result) + ", " + StrOf(Vector) + ", " + StrOf(Component) + ", " + StrOf(Index) + ")";
        public override string ArgString => "Vector: " + StrOf(Vector) + ", " + "Component: " + StrOf(Component) + ", " + "Index: " + StrOf(Index);

        protected override void FromCode(uint[] codes, int start)
        {
            System.Diagnostics.Debug.Assert((codes[start] & 0x0000FFFF) == (uint)OpCode.VectorInsertDynamic);
            var i = start + 1;
            ResultType = new ID(codes[i++]);
            Result = new ID(codes[i++]);
            Vector = new ID(codes[i++]);
            Component = new ID(codes[i++]);
            Index = new ID(codes[i++]);
        }

        protected override void WriteCode(List<uint> code)
        {
            code.Add(ResultType.Value);
            code.Add(Result.Value);
            code.Add(Vector.Value);
            code.Add(Component.Value);
            code.Add(Index.Value);
        }

        public override IEnumerable<ID> AllIDs
        {
            get
            {
                yield return ResultType;
                yield return Result;
                yield return Vector;
                yield return Component;
                yield return Index;
            }
        }
        #endregion
    }
}
