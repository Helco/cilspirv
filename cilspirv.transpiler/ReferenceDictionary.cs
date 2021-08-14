using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace cilspirv
{
    internal class ReferenceDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : class
    {
        public ReferenceDictionary() : base(new ReferenceComparer<TKey>()) { }
    }

    internal class ReferenceComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
        public int GetHashCode(T? obj) => obj?.GetHashCode() ?? 0;
    }
}
