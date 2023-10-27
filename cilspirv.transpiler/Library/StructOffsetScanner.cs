using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using cilspirv.Spirv;
using cilspirv.Transpiler;

namespace cilspirv.Library
{
    internal class StructOffsetScanner : NullTranspilerLibraryMapper
    {
        public override IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider attributeProvider, IDecorationContext? context)
        {
            if (attributeProvider is not FieldDefinition fieldDef ||
                context is not IFieldDecorationContext fieldContext ||
                !fieldContext.IsValueStruct ||
                fieldDef.Offset < 0)
                return Enumerable.Empty<DecorationEntry>();

            return new[] { Decorations.Offset((uint)fieldDef.Offset) };
        }
    }
}
