using System;
using System.Collections.Generic;
using static cilspirv.Spirv.DecorationTargetKinds;

namespace cilspirv.Spirv
{
    [Flags]
    public enum DecorationTargetKinds
    {
        Unknown = 0,
        Variable = (1 << 0),
        Parameter = (1 << 1),
        Function = (1 << 2),
        FunctionCall = (1 << 3),
        Instruction = (1 << 4),
        Member = (1 << 5),
        Structure = (1 << 6),
        ArrayType = (1 << 7),
        SpecConstant = (1 << 7),
        Constant = (1 << 8),
        Pointer = (1 << 9)
    }

    partial class Decorations
    {
        public static DecorationTargetKinds GetTargetKinds(this Decoration decoration) =>
            targetKinds.TryGetValue(decoration, out var kinds) ? kinds : default;

#pragma warning disable CS0618
        private static readonly Dictionary<Decoration, DecorationTargetKinds> targetKinds = new Dictionary<Decoration, DecorationTargetKinds>()
        {
            { Decoration.RelaxedPrecision, Variable | Parameter | Function | FunctionCall | Instruction },
            { Decoration.SpecId, SpecConstant },
            { Decoration.Block, Structure },
            { Decoration.BufferBlock, Structure },
            { Decoration.RowMajor, Member },
            { Decoration.ColMajor, Member },
            { Decoration.ArrayStride, ArrayType },
            { Decoration.MatrixStride, Member },
            { Decoration.GLSLShared, Structure },
            { Decoration.GLSLPacked, Structure },
            { Decoration.CPacked, Structure },
            { Decoration.BuiltIn, Variable | Member | DecorationTargetKinds.Constant },
            { Decoration.NoPerspective, Member | Variable | Parameter },
            { Decoration.Flat, Member | Variable | Parameter },
            { Decoration.Patch, Member | Variable | Parameter },
            { Decoration.Centroid, Member | Variable | Parameter },
            { Decoration.Sample, Member | Variable | Parameter },
            { Decoration.Invariant, Variable | Member },
            { Decoration.Restrict, Variable | Parameter },
            { Decoration.Aliased, Variable | Parameter },
            { Decoration.Volatile, Member | Variable | Parameter },
            { Decoration.Constant, Variable },
            { Decoration.Coherent, Member | Variable | Parameter },
            { Decoration.NonWritable, Member | Variable | Parameter },
            { Decoration.NonReadable, Member | Variable | Parameter },
            { Decoration.Uniform, Variable },
            { Decoration.UniformId, Variable },
            { Decoration.SaturatedConversion, Instruction },
            { Decoration.Stream, Member | Variable | Parameter },
            { Decoration.Location, Variable | Member },
            { Decoration.Component, Member | Variable | Parameter },
            { Decoration.Index, Variable },
            { Decoration.Binding, Variable },
            { Decoration.DescriptorSet, Variable },
            { Decoration.Offset, Member },
            { Decoration.XfbBuffer, Member | Variable | Parameter },
            { Decoration.XfbStride, Member | Variable | Parameter },
            { Decoration.FuncParamAttr, Parameter },
            // skipped FPRoundingMode, FPFastMathMode
            { Decoration.LinkageAttributes, Function | Variable },
            { Decoration.NoContraction, Instruction },
            { Decoration.InputAttachmentIndex, Variable },
            { Decoration.Alignment, Pointer },
            { Decoration.MaxByteOffset, Pointer },
            { Decoration.AlignmentId, Pointer },
            { Decoration.MaxByteOffsetId, Pointer },
            { Decoration.NoSignedWrap, Instruction },
            { Decoration.NoUnsignedWrap, Instruction },
            // skipped vendor-specific decorations without documentation
            { Decoration.NonUniform, Variable },
            { Decoration.RestrictPointer, Variable | Parameter },
            { Decoration.AliasedPointer, Variable | Parameter },
            { Decoration.CounterBuffer, Variable },
            { Decoration.UserSemantic, Variable | Member }
        };
    }
}
