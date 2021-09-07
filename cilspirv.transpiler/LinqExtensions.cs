using System;
using System.Collections.Generic;
using System.Linq;

namespace cilspirv
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> setOfSets) =>
            setOfSets.SelectMany(set => set);

        /// <summary>Like the normal SelectMany but takes an <see cref="IEnumerator{T}"/> instead</summary>
        /// <remarks>Not thread-safe</remarks>
        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerator<T>> setOfSets) =>
            setOfSets.SelectMany(EnumeratorAsEnumerable);

        /// <summary>Like the normal SelectMany but takes an <see cref="IEnumerator{T}"/> instead</summary>
        /// <remarks>Not thread-safe</remarks>
        public static IEnumerable<T> SelectMany<TSource, T>(this IEnumerable<TSource> setOfSets, Func<TSource, IEnumerator<T>> getEnumerator) =>
            setOfSets.SelectMany(source => EnumeratorAsEnumerable(getEnumerator(source)));

        private static IEnumerable<T> EnumeratorAsEnumerable<T>(IEnumerator<T> enumerator)
        {
            if (enumerator.Current != null)
                enumerator.Reset();
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        public static int IndexOf<T>(this IEnumerable<T> set, T search, IEqualityComparer<T>? comparer = null)
        {
            comparer ??= EqualityComparer<T>.Default;
            int searchHash = search == null ? 0 : comparer.GetHashCode(search);
            int i = 0;
            foreach (var elem in set)
            {
                int elemHash = elem == null ? 0 : comparer.GetHashCode(elem);
                if (elemHash == searchHash && comparer.Equals(search, elem))
                    return i;
                i++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> set, Predicate<T> predicate)
        {
            int i = 0;
            foreach (var elem in set)
            {
                if (predicate(elem))
                    return i;
                i++;
            }
            return -1;
        }
    }
}
