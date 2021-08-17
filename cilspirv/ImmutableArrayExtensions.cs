using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace cilspirv
{
    public static class ImmutableArrayExtensions
    {
        public static bool ValueEquals<T>(this ImmutableArray<T> a, ImmutableArray<T> b, IEqualityComparer<T>? comparer = null)
        {
            if (a.IsDefaultOrEmpty != b.IsDefaultOrEmpty)
                return false;
            if (a.IsDefaultOrEmpty)
                return true;

            comparer ??= EqualityComparer<T>.Default;
            for (int i = 0; i < a.Length; i++)
                if (!comparer.Equals(a[i], b[i]))
                    return false;
            return true;
        }
    }
}
