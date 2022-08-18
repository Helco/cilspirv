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
        GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;
        IMappedFromCILType? TryMapType(TypeReference ilTypeRef) => null;
        IValueBehaviour? TryMapFieldBehavior(FieldReference fieldRef) => null;
        StorageClass? TryScanStorageClass(ICustomAttributeProvider fieldDef) => null;
        IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider field, IDecorationContext? context) => Enumerable.Empty<DecorationEntry>();
    }
}
