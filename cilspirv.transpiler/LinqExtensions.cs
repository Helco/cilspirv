using System;
using System.Collections.Generic;
using System.Linq;

namespace cilspirv
{
#if !NETSTANDARD2_1_OR_GREATER
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool _) { }
    }

#endif

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

        public static IEnumerable<IEnumerable<T>> SegmentsByLast<T>(this IEnumerable<T> set, Predicate<T> isLast)
        {
            int start = 0, i = 0;
            foreach (var elem in set)
            {
                if (isLast(elem))
                {
                    yield return set.Skip(start).Take(i - start + 1);
                    start = i + 1;
                }
                i++;
            }
            if (i > start)
                yield return set.Skip(start);
        }

        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            value = default!;
            if (!queue.Any())
                return false;
            value = queue.Dequeue();
            return true;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> set, int count)
        {
            if (count <= 0)
                return Enumerable.Empty<T>();
            if (set is IReadOnlyCollection<T> coll)
            {
                var skip = Math.Max(0, coll.Count - count);
                return coll.Skip(skip);
            }
            return set.Reverse().Take(count).Reverse();
        }

#if !NETSTANDARD2_1_OR_GREATER
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> set) => new HashSet<T>(set);
#endif
    }
}
