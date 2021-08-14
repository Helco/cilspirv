using System;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal class TranspilerVariable
    {
        public string Name { get; }
        public SpirvPointerType PointerType { get; }
        public SpirvType ElementType => PointerType.Type ?? throw new InvalidOperationException("Element type is not set");
        public StorageClass Storage => PointerType.Storage;

        public TranspilerVariable(string name, SpirvPointerType pointerType) =>
            (Name, PointerType) = (name, pointerType);
    }
}