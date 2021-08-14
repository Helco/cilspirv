// This file was generated. Do not modify.
using System;

namespace cilspirv.Spirv
{
    public static class Decorations
    {
        public static DecorationEntry RelaxedPrecision() => new DecorationEntry(Decoration.RelaxedPrecision);
        public static DecorationEntry SpecId(uint specID) => new DecorationEntry(Decoration.SpecId, specID);
        public static DecorationEntry Block() => new DecorationEntry(Decoration.Block);
        [Obsolete("Last version for this enumerant was 1.3")]
        public static DecorationEntry BufferBlock() => new DecorationEntry(Decoration.BufferBlock);
        public static DecorationEntry RowMajor() => new DecorationEntry(Decoration.RowMajor);
        public static DecorationEntry ColMajor() => new DecorationEntry(Decoration.ColMajor);
        public static DecorationEntry ArrayStride(uint stride) => new DecorationEntry(Decoration.ArrayStride, stride);
        public static DecorationEntry MatrixStride(uint stride) => new DecorationEntry(Decoration.MatrixStride, stride);
        public static DecorationEntry GLSLShared() => new DecorationEntry(Decoration.GLSLShared);
        public static DecorationEntry GLSLPacked() => new DecorationEntry(Decoration.GLSLPacked);
        public static DecorationEntry CPacked() => new DecorationEntry(Decoration.CPacked);
        public static DecorationEntry BuiltIn(BuiltIn builtIn) => new DecorationEntry(Decoration.BuiltIn, builtIn);
        public static DecorationEntry NoPerspective() => new DecorationEntry(Decoration.NoPerspective);
        public static DecorationEntry Flat() => new DecorationEntry(Decoration.Flat);
        public static DecorationEntry Patch() => new DecorationEntry(Decoration.Patch);
        public static DecorationEntry Centroid() => new DecorationEntry(Decoration.Centroid);
        public static DecorationEntry Sample() => new DecorationEntry(Decoration.Sample);
        public static DecorationEntry Invariant() => new DecorationEntry(Decoration.Invariant);
        public static DecorationEntry Restrict() => new DecorationEntry(Decoration.Restrict);
        public static DecorationEntry Aliased() => new DecorationEntry(Decoration.Aliased);
        public static DecorationEntry Volatile() => new DecorationEntry(Decoration.Volatile);
        public static DecorationEntry Constant() => new DecorationEntry(Decoration.Constant);
        public static DecorationEntry Coherent() => new DecorationEntry(Decoration.Coherent);
        public static DecorationEntry NonWritable() => new DecorationEntry(Decoration.NonWritable);
        public static DecorationEntry NonReadable() => new DecorationEntry(Decoration.NonReadable);
        public static DecorationEntry Uniform() => new DecorationEntry(Decoration.Uniform);
        public static DecorationEntry UniformId(ID scope) => new DecorationEntry(Decoration.UniformId, scope);
        public static DecorationEntry SaturatedConversion() => new DecorationEntry(Decoration.SaturatedConversion);
        public static DecorationEntry Stream(uint stream) => new DecorationEntry(Decoration.Stream, stream);
        public static DecorationEntry Location(uint location) => new DecorationEntry(Decoration.Location, location);
        public static DecorationEntry Component(uint component) => new DecorationEntry(Decoration.Component, component);
        public static DecorationEntry Index(uint index) => new DecorationEntry(Decoration.Index, index);
        public static DecorationEntry Binding(uint binding) => new DecorationEntry(Decoration.Binding, binding);
        public static DecorationEntry DescriptorSet(uint descriptorSet) => new DecorationEntry(Decoration.DescriptorSet, descriptorSet);
        public static DecorationEntry Offset(uint offset) => new DecorationEntry(Decoration.Offset, offset);
        public static DecorationEntry XfbBuffer(uint buffer) => new DecorationEntry(Decoration.XfbBuffer, buffer);
        public static DecorationEntry XfbStride(uint stride) => new DecorationEntry(Decoration.XfbStride, stride);
        public static DecorationEntry FuncParamAttr(FunctionParameterAttribute attr) => new DecorationEntry(Decoration.FuncParamAttr, attr);
        public static DecorationEntry FPRoundingMode(FPRoundingMode mode) => new DecorationEntry(Decoration.FPRoundingMode, mode);
        public static DecorationEntry FPFastMathMode(FPFastMathMode mode) => new DecorationEntry(Decoration.FPFastMathMode, mode);
        public static DecorationEntry LinkageAttributes(string name, LinkageType type) => new DecorationEntry(Decoration.LinkageAttributes, name, type);
        public static DecorationEntry NoContraction() => new DecorationEntry(Decoration.NoContraction);
        public static DecorationEntry InputAttachmentIndex(uint index) => new DecorationEntry(Decoration.InputAttachmentIndex, index);
        public static DecorationEntry Alignment(uint alignment) => new DecorationEntry(Decoration.Alignment, alignment);
        public static DecorationEntry MaxByteOffset(uint offset) => new DecorationEntry(Decoration.MaxByteOffset, offset);
        public static DecorationEntry AlignmentId(ID id) => new DecorationEntry(Decoration.AlignmentId, id);
        public static DecorationEntry MaxByteOffsetId(ID id) => new DecorationEntry(Decoration.MaxByteOffsetId, id);
        public static DecorationEntry NoSignedWrap() => new DecorationEntry(Decoration.NoSignedWrap);
        public static DecorationEntry NoUnsignedWrap() => new DecorationEntry(Decoration.NoUnsignedWrap);
        public static DecorationEntry ExplicitInterpAMD() => new DecorationEntry(Decoration.ExplicitInterpAMD);
        public static DecorationEntry OverrideCoverageNV() => new DecorationEntry(Decoration.OverrideCoverageNV);
        public static DecorationEntry PassthroughNV() => new DecorationEntry(Decoration.PassthroughNV);
        public static DecorationEntry ViewportRelativeNV() => new DecorationEntry(Decoration.ViewportRelativeNV);
        public static DecorationEntry SecondaryViewportRelativeNV() => new DecorationEntry(Decoration.SecondaryViewportRelativeNV);
        public static DecorationEntry PerPrimitiveNV() => new DecorationEntry(Decoration.PerPrimitiveNV);
        public static DecorationEntry PerViewNV() => new DecorationEntry(Decoration.PerViewNV);
        public static DecorationEntry PerTaskNV() => new DecorationEntry(Decoration.PerTaskNV);
        public static DecorationEntry PerVertexNV() => new DecorationEntry(Decoration.PerVertexNV);
        public static DecorationEntry NonUniform() => new DecorationEntry(Decoration.NonUniform);
        public static DecorationEntry NonUniformEXT() => new DecorationEntry(Decoration.NonUniformEXT);
        public static DecorationEntry RestrictPointer() => new DecorationEntry(Decoration.RestrictPointer);
        public static DecorationEntry RestrictPointerEXT() => new DecorationEntry(Decoration.RestrictPointerEXT);
        public static DecorationEntry AliasedPointer() => new DecorationEntry(Decoration.AliasedPointer);
        public static DecorationEntry AliasedPointerEXT() => new DecorationEntry(Decoration.AliasedPointerEXT);
        public static DecorationEntry SIMTCallINTEL() => new DecorationEntry(Decoration.SIMTCallINTEL);
        public static DecorationEntry ReferencedIndirectlyINTEL() => new DecorationEntry(Decoration.ReferencedIndirectlyINTEL);
        public static DecorationEntry ClobberINTEL() => new DecorationEntry(Decoration.ClobberINTEL);
        public static DecorationEntry SideEffectsINTEL() => new DecorationEntry(Decoration.SideEffectsINTEL);
        public static DecorationEntry VectorComputeVariableINTEL() => new DecorationEntry(Decoration.VectorComputeVariableINTEL);
        public static DecorationEntry FuncParamIOKindINTEL() => new DecorationEntry(Decoration.FuncParamIOKindINTEL);
        public static DecorationEntry VectorComputeFunctionINTEL() => new DecorationEntry(Decoration.VectorComputeFunctionINTEL);
        public static DecorationEntry StackCallINTEL() => new DecorationEntry(Decoration.StackCallINTEL);
        public static DecorationEntry GlobalVariableOffsetINTEL() => new DecorationEntry(Decoration.GlobalVariableOffsetINTEL);
        public static DecorationEntry CounterBuffer() => new DecorationEntry(Decoration.CounterBuffer);
        public static DecorationEntry HlslCounterBufferGOOGLE() => new DecorationEntry(Decoration.HlslCounterBufferGOOGLE);
        public static DecorationEntry UserSemantic() => new DecorationEntry(Decoration.UserSemantic);
        public static DecorationEntry HlslSemanticGOOGLE() => new DecorationEntry(Decoration.HlslSemanticGOOGLE);
        public static DecorationEntry UserTypeGOOGLE() => new DecorationEntry(Decoration.UserTypeGOOGLE);
        public static DecorationEntry FunctionRoundingModeINTEL() => new DecorationEntry(Decoration.FunctionRoundingModeINTEL);
        public static DecorationEntry FunctionDenormModeINTEL() => new DecorationEntry(Decoration.FunctionDenormModeINTEL);
        public static DecorationEntry RegisterINTEL() => new DecorationEntry(Decoration.RegisterINTEL);
        public static DecorationEntry MemoryINTEL() => new DecorationEntry(Decoration.MemoryINTEL);
        public static DecorationEntry NumbanksINTEL() => new DecorationEntry(Decoration.NumbanksINTEL);
        public static DecorationEntry BankwidthINTEL() => new DecorationEntry(Decoration.BankwidthINTEL);
        public static DecorationEntry MaxPrivateCopiesINTEL() => new DecorationEntry(Decoration.MaxPrivateCopiesINTEL);
        public static DecorationEntry SinglepumpINTEL() => new DecorationEntry(Decoration.SinglepumpINTEL);
        public static DecorationEntry DoublepumpINTEL() => new DecorationEntry(Decoration.DoublepumpINTEL);
        public static DecorationEntry MaxReplicatesINTEL() => new DecorationEntry(Decoration.MaxReplicatesINTEL);
        public static DecorationEntry SimpleDualPortINTEL() => new DecorationEntry(Decoration.SimpleDualPortINTEL);
        public static DecorationEntry MergeINTEL() => new DecorationEntry(Decoration.MergeINTEL);
        public static DecorationEntry BankBitsINTEL() => new DecorationEntry(Decoration.BankBitsINTEL);
        public static DecorationEntry ForcePow2DepthINTEL() => new DecorationEntry(Decoration.ForcePow2DepthINTEL);
        public static DecorationEntry BurstCoalesceINTEL() => new DecorationEntry(Decoration.BurstCoalesceINTEL);
        public static DecorationEntry CacheSizeINTEL() => new DecorationEntry(Decoration.CacheSizeINTEL);
        public static DecorationEntry DontStaticallyCoalesceINTEL() => new DecorationEntry(Decoration.DontStaticallyCoalesceINTEL);
        public static DecorationEntry PrefetchINTEL() => new DecorationEntry(Decoration.PrefetchINTEL);
        public static DecorationEntry StallEnableINTEL() => new DecorationEntry(Decoration.StallEnableINTEL);
        public static DecorationEntry FuseLoopsInFunctionINTEL() => new DecorationEntry(Decoration.FuseLoopsInFunctionINTEL);
        public static DecorationEntry BufferLocationINTEL() => new DecorationEntry(Decoration.BufferLocationINTEL);
        public static DecorationEntry IOPipeStorageINTEL() => new DecorationEntry(Decoration.IOPipeStorageINTEL);
        public static DecorationEntry FunctionFloatingPointModeINTEL() => new DecorationEntry(Decoration.FunctionFloatingPointModeINTEL);
        public static DecorationEntry SingleElementVectorINTEL() => new DecorationEntry(Decoration.SingleElementVectorINTEL);
        public static DecorationEntry VectorComputeCallableFunctionINTEL() => new DecorationEntry(Decoration.VectorComputeCallableFunctionINTEL);
    }
}

