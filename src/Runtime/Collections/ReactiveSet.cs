using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal class ReactiveSet<T> : IReactiveCollection, ISet<T>, IReadOnlyCollection<T>
    {
        private readonly HashSet<T> _set = new();

        public bool IsDisposed { get; private set; }

        public event Action<IReactiveCollection> OnChanged = delegate { };

        private void SetDirty()
        {
            OnChanged(this);
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ReactiveSet<T>));
        }

        #region Mutable functionallity

        public void Dispose()
        {
            if (IsDisposed)
                return;
            Clear();
            IsDisposed = true;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            ThrowIfDisposed();
            var count = Count;
            _set.ExceptWith(other);
            if (Count != count)
                SetDirty();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            ThrowIfDisposed();
            var count = Count;
            _set.IntersectWith(other);
            if (Count != count)
                SetDirty();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ThrowIfDisposed();
            var count = Count;
            _set.SymmetricExceptWith(other);
            if (Count != count)
                SetDirty();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            ThrowIfDisposed();
            var count = Count;
            _set.UnionWith(other);
            if (Count != count)
                SetDirty();
        }

        void ICollection<T>.Add(T? item)
        {
            Add(item);
        }

        public bool Add(T? item)
        {
            ThrowIfDisposed();
#pragma warning disable CS8604 // Possible null reference argument.
            var added = _set.Add(item);
#pragma warning restore CS8604 // Possible null reference argument.
            if (added)
                SetDirty();
            return added;
        }

        public void Clear()
        {
            ThrowIfDisposed();
            var count = Count;
            _set.Clear();
            if (Count != count)
                SetDirty();
        }

        public bool Remove(T? item)
        {
            ThrowIfDisposed();
#pragma warning disable CS8604 // Possible null reference argument.
            var removed = _set.Remove(item);
#pragma warning restore CS8604 // Possible null reference argument.
            if (removed)
                SetDirty();
            return removed;
        }

        #endregion

        #region Immutable functionallity

        public int Count => _set.Count;

        public bool IsReadOnly => IsDisposed || ((ISet<T>)_set).IsReadOnly;

        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _set.SetEquals(other);
        }

        public bool Contains(T? item)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return _set.Contains(item);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        #endregion
    }
}