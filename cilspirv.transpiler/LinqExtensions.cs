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
    }
}
