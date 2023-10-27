using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;

namespace cilspirv.Transpiler
{
    internal interface IMappedFromCILType { }

    internal interface IDecorationContext { }
    internal interface IFieldDecorationContext : IDecorationContext
    {
        bool IsValueStruct { get; }
        bool IsVarGroup { get; }
    }

    internal interface ITranspilerLibraryMapper
    {
        GenerateCallDelegate? TryMapMethod(MethodReference methodRef);
        IMappedFromCILType? TryMapType(TypeReference ilTypeRef);
        IValueBehaviour? TryMapFieldBehavior(FieldReference fieldRef);
        StorageClass? TryScanStorageClass(ICustomAttributeProvider fieldDef);
        IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider field, IDecorationContext? context);
    }

    internal class NullTranspilerLibraryMapper : ITranspilerLibraryMapper
    {
        public virtual GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;
        public virtual IMappedFromCILType? TryMapType(TypeReference ilTypeRef) => null;
        public virtual IValueBehaviour? TryMapFieldBehavior(FieldReference fieldRef) => null;
        public virtual StorageClass? TryScanStorageClass(ICustomAttributeProvider fieldDef) => null;
        public virtual IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider field, IDecorationContext? context) => Enumerable.Empty<DecorationEntry>();
    }
}
