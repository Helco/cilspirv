using System.Collections;
using System.Collections.Generic;

namespace cilspirv
{
    internal class ObservableHashSet<T> : ObservableSet<T>
    {
        public ObservableHashSet() : base(new HashSet<T>()) { }
    }

    internal class ObservableSet<T> : ISet<T>
    {
        private readonly ISet<T> set;
        private bool hasChanged = true;

        /// <summary>Resets the changed flag and returns its previous state</summary>
        public bool ResetHasChanged()
        {
            var result = hasChanged;
            hasChanged = false;
            return result;
        }

        public ObservableSet(ISet<T> set) => this.set = set;

        public int Count => set.Count;
        public bool IsReadOnly => set.IsReadOnly;

        public bool Add(T item) => hasChanged |= set.Add(item);
        void ICollection<T>.Add(T item) => hasChanged |= Add(item);
        public bool Remove(T item) => hasChanged |= set.Remove(item);

        public void Clear()
        {
            hasChanged |= set.Count > 0;
            set.Clear();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            hasChanged = true;
            set.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            hasChanged = true;
            set.IntersectWith(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            hasChanged = true;
            set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            hasChanged = true;
            set.UnionWith(other);
        }

        public void CopyTo(T[] array, int arrayIndex) => set.CopyTo(array, arrayIndex);
        public bool Contains(T item) => set.Contains(item);
        public bool SetEquals(IEnumerable<T> other) => set.SetEquals(other);
        public bool IsProperSubsetOf(IEnumerable<T> other) => set.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<T> other) => set.IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<T> other) => set.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<T> other) => set.IsSupersetOf(other);
        public bool Overlaps(IEnumerable<T> other) => set.Overlaps(other);
        public IEnumerator<T> GetEnumerator() => set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)set).GetEnumerator();
    }
}