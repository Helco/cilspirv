using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using cilspirv.Transpiler;
using Mono.Cecil;

namespace cilspirv.Library
{
    internal class ExternalTypeMapper : ITranspilerLibraryMapper, IEnumerable<KeyValuePair<Type, SpirvType>>
    {
        private readonly Dictionary<Type, SpirvType> mappedTypes = new Dictionary<Type, SpirvType>();
        private readonly Dictionary<string, SpirvType> typeByName = new Dictionary<string, SpirvType>();
        private readonly Dictionary<string, IValueBehaviour> fields = new Dictionary<string, IValueBehaviour>();
        private readonly Dictionary<string, List<DecorationEntry>> typeDecorations = new Dictionary<string, List<DecorationEntry>>();

        public void Add(Type ilType, SpirvType spirvType)
        {
            if (spirvType is SpirvStructType)
                throw new ArgumentException("Struct types cannot be mapped as external type");
            if (spirvType.UserName == null)
                spirvType = spirvType with { UserName = ilType.FullName };
            mappedTypes.Add(ilType, spirvType);
            typeByName.Add(ilType.FullName ?? throw new ArgumentNullException("IL type does not have a name"), spirvType);
        }

        public IMappedFromCILType? TryMapType(TypeReference ilTypeRef) =>
            typeByName.TryGetValue(ilTypeRef.FullName, out var spirvType)
            ? spirvType
            : null;

        public IEnumerator<KeyValuePair<Type, SpirvType>> GetEnumerator() => mappedTypes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;

        public IValueBehaviour? TryMapFieldBehavior(FieldReference fieldRef) =>
            fields.TryGetValue(fieldRef.FullName, out var fieldBehaviour)
            ? fieldBehaviour
            : null;

        public IEnumerable<DecorationEntry> TryScanDecorations(ICustomAttributeProvider provider, IDecorationContext? _)
        {
            var relatedType = provider.GetRelatedType();
            if (relatedType?.IsByReference ?? false)
                relatedType = relatedType.GetElementType();
            return typeDecorations.TryGetValue(relatedType?.FullName ?? "", out var decorations)
                ? decorations
                : Enumerable.Empty<DecorationEntry>();
        }

        public void Add<TParent, TField>(string fieldName, IValueBehaviour behavior) => Add(typeof(TParent), typeof(TField), fieldName, behavior);
        public void Add(Type ilParentType, Type ilFieldType, string fieldName, IValueBehaviour behavior)
        {
            var mappingName = $"{ilFieldType.FullName} {ilParentType.FullName}::{fieldName}";
            fields.Add(mappingName, behavior);
        }

        public void Add<TType>(params DecorationEntry[] decorations) => Add(typeof(TType), decorations);
        public void Add(Type type, params DecorationEntry[] decorations)
        {
            if (!typeDecorations.TryGetValue(type.FullName!, out var decorationList))
                typeDecorations.Add(type.FullName!, decorationList = new List<DecorationEntry>());

            decorationList.AddRange(decorations);
        }
    }
}
