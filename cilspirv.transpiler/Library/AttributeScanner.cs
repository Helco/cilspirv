using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Transpiler;
using cilspirv.Spirv;

namespace cilspirv.Library
{
    internal class AttributeScanner : IITranspilerLibraryScanner
    {
        public StorageClass? TryScanStorageClass(ICustomAttributeProvider element)
        {
            var attr = element.GetCustomAttributes<StorageClassAttributeBase>(exactType: false).SingleOrDefault();
            if (attr == null)
                return null;
            return attr.Instantiate<StorageClassAttributeBase>().StorageClass;
        }

        public IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider element) => element
            .GetCustomAttributes<DecorationAttributeBase>(exactType: false)
            .SelectMany(attr => attr.Instantiate<DecorationAttributeBase>().Decorations);

        public StorageClass? TryScanStorageClass(FieldReference fieldRef) => TryScanStorageClass(fieldRef.Resolve() as ICustomAttributeProvider);
        public StorageClass? TryScanStorageClass(ParameterReference parameterRef) => TryScanStorageClass(parameterRef.Resolve() as ICustomAttributeProvider);
        public StorageClass? TryScanStorageClass(MethodReturnType returnType) => TryScanStorageClass(returnType as ICustomAttributeProvider);
        public IEnumerable<DecorationEntry> TryScanDecorations(FieldReference fieldRef) => TryScanDecorations(fieldRef.Resolve() as ICustomAttributeProvider);
        public IEnumerable<DecorationEntry> TryScanDecorations(ParameterReference parameterRef) => TryScanDecorations(parameterRef.Resolve() as ICustomAttributeProvider);
        public IEnumerable<DecorationEntry> TryScanDecorations(MethodReturnType returnType) => TryScanDecorations(returnType as ICustomAttributeProvider);
    }
}
