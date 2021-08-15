﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cilspirv.Spirv;
using Mono.Cecil;

namespace cilspirv.Transpiler
{
    internal class TranspilerExternalTypeMapper : ITranspilerLibraryMapper, IEnumerable<KeyValuePair<Type, SpirvType>>
    {
        private readonly Dictionary<Type, SpirvType> mappedTypes = new Dictionary<Type, SpirvType>();
        private readonly Dictionary<string, SpirvType> typeByName = new Dictionary<string, SpirvType>();

        public void Add(Type ilType, SpirvType spirvType)
        {
            if (spirvType is SpirvStructType)
                throw new ArgumentException("Struct types cannot be mapped as external type");
            mappedTypes.Add(ilType, spirvType);
            typeByName.Add(ilType.FullName ?? throw new ArgumentNullException("IL type does not have a name"), spirvType);
        }

        public TranspilerType? TryMapType(TypeReference ilTypeRef) =>
            typeByName.TryGetValue(ilTypeRef.FullName, out var spirvType)
            ? new TranspilerType(name: null, spirvType)
            : null;

        public IEnumerator<KeyValuePair<Type, SpirvType>> GetEnumerator() => mappedTypes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public GenerateCallDelegate? TryMapMethod(MethodReference methodRef) => null;
    }
}