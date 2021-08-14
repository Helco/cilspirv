using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace cilspirv
{
    public abstract class BiDictionaryInner<T1, T2> : IDictionary<T1, T2> where T1 : notnull
    {
        public abstract T2 this[T1 key] { get; set; }

        public abstract ICollection<T1> Keys { get; }
        public abstract ICollection<T2> Values { get; }
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        public abstract void Add(T1 key, T2 value);
        public abstract void Add(KeyValuePair<T1, T2> item);
        public abstract void Clear();
        public abstract bool Contains(KeyValuePair<T1, T2> item);
        public abstract bool ContainsKey(T1 key);
        public abstract void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex);
        public abstract IEnumerator<KeyValuePair<T1, T2>> GetEnumerator();
        public abstract bool Remove(T1 key);
        public abstract bool Remove(KeyValuePair<T1, T2> item);
        public abstract bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value);

        IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException("Ambiguous enumerator cannot be created");
    }

    public class BiDictionary<T1, T2> : BiDictionaryInner<T1, T2>, IDictionary<T2, T1>
        where T1 : notnull
        where T2 : notnull
    {
        private readonly IDictionary<T1, T2> forward;
        private readonly IDictionary<T2, T1> backward;

        public BiDictionary(IEqualityComparer<T1>? comparer1 = null, IEqualityComparer<T2>? comparer2 = null)
        {
            forward = new Dictionary<T1, T2>(comparer1 ?? EqualityComparer<T1>.Default);
            backward = new Dictionary<T2, T1>(comparer2 ?? EqualityComparer<T2>.Default);
        }

        public override T2 this[T1 key]
        {
            get => forward[key];
            set
            {
                if (TryGetValue(key, out var prevValue))
                    backward.Remove(prevValue);
                forward[key] = value;
                backward[value] = key;
            }
        }

        public T1 this[T2 key]
        {
            get => backward[key];
            set
            {
                if (TryGetValue(key, out var prevValue))
                    forward.Remove(prevValue);
                backward[key] = value;
                forward[value] = key;
            }
        }

        public override bool IsReadOnly => false;
        public override int Count => forward.Count;
        public override ICollection<T1> Keys => forward.Keys;
        public override ICollection<T2> Values => forward.Values;
        ICollection<T2> IDictionary<T2, T1>.Keys => backward.Keys;
        ICollection<T1> IDictionary<T2, T1>.Values => backward.Values;

        public override IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => forward.GetEnumerator();
        IEnumerator<KeyValuePair<T2, T1>> IEnumerable<KeyValuePair<T2, T1>>.GetEnumerator() => backward.GetEnumerator();
        public override bool Contains(KeyValuePair<T1, T2> item) => forward.Contains(item);
        public bool Contains(KeyValuePair<T2, T1> item) => backward.Contains(item);
        public override bool ContainsKey(T1 key) => forward.ContainsKey(key);
        public bool ContainsKey(T2 key) => backward.ContainsKey(key);
        public override void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) => forward.CopyTo(array, arrayIndex);
        public void CopyTo(KeyValuePair<T2, T1>[] array, int arrayIndex) => backward.CopyTo(array, arrayIndex);
        public override bool Remove(KeyValuePair<T1, T2> item) => Remove(item.Key);
        public bool Remove(KeyValuePair<T2, T1> item) => Remove(item.Key);
        public override bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value) => forward.TryGetValue(key, out value);
        public bool TryGetValue(T2 key, [MaybeNullWhen(false)] out T1 value) => backward.TryGetValue(key, out value);

        public override void Clear()
        {
            forward.Clear();
            backward.Clear();
        }

        public override void Add(T1 key, T2 value)
        {
            forward.Add(key, value);
            backward.Add(value, key);
        }

        public void Add(T2 key, T1 value)
        {
            forward.Add(value, key);
            backward.Add(key, value);
        }

        public override void Add(KeyValuePair<T1, T2> item)
        {
            forward.Add(item);
            backward.Add(item.Value, item.Key);
        }

        public void Add(KeyValuePair<T2, T1> item)
        {
            forward.Add(item.Value, item.Key);
            backward.Add(item);
        }

        public override bool Remove(T1 key)
        {
            if (!TryGetValue(key, out var value))
                return false;
            forward.Remove(key);
            backward.Remove(value);
            return true;
        }

        public bool Remove(T2 key)
        {
            if (!TryGetValue(key, out var value))
                return false;
            forward.Remove(value);
            backward.Remove(key);
            return true;
        }
    }
}
