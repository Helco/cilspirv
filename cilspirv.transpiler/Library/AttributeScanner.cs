 using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Transpiler;
using cilspirv.Spirv;

namespace cilspirv.Library
{
    internal class AttributeScanner : NullTranspilerLibraryMapper
    {
        public override StorageClass? TryScanStorageClass(ICustomAttributeProvider element)
        {
            var attr = element.GetCustomAttributes<StorageClassAttribute>(exactType: false).SingleOrDefault();
            if (attr == null)
                return null;
            return attr.Instantiate<StorageClassAttribute>().StorageClass;
        }

        public override IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider element, IDecorationContext? _) => element
            .GetCustomAttributes<DecorationAttribute>(exactType: false)
            .SelectMany(attr => attr.Instantiate<DecorationAttribute>().Decorations);
    }
}
