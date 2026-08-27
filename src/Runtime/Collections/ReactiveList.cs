using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal class ReactiveList<T> : IReactiveCollection, IList<T>, IReadOnlyList<T>
    {
        private readonly List<T> _list = new();

        public bool IsDisposed { get; private set; }

        public event Action<IReactiveCollection> OnChanged = delegate { };

        private void SetDirty()
        {
            OnChanged(this);
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ReactiveList<T>));
        }

        #region Mutable functionallity

        public void Dispose()
        {
            if (IsDisposed)
                return;
            Clear();
            IsDisposed = true;
        }

        public T this[int index]
        {
            get
            {
                ThrowIfDisposed();
                return _list[index];
            }
            set
            {
                ThrowIfDisposed();
                _list[index] = value;
                SetDirty();
            }
        }

        public void Add(T? item)
        {
            ThrowIfDisposed();
#pragma warning disable CS8604 // Possible null reference argument.
            _list.Add(item);
#pragma warning restore CS8604 // Possible null reference argument.
            SetDirty();
        }

        public void Clear()
        {
            ThrowIfDisposed();
            if (_list.Count > 0)
            {
                _list.Clear();
                SetDirty();
            }
        }

        public bool Remove(T? item)
        {
            ThrowIfDisposed();
#pragma warning disable CS8604 // Possible null reference argument.
            var removed = _list.Remove(item);
#pragma warning restore CS8604 // Possible null reference argument.
            if (removed)
                SetDirty();
            return removed;
        }

        public void Insert(int index, T? item)
        {
            ThrowIfDisposed();
#pragma warning disable CS8604 // Possible null reference argument.
            _list.Insert(index, item);
#pragma warning restore CS8604 // Possible null reference argument.
            SetDirty();
        }

        public void RemoveAt(int index)
        {
            ThrowIfDisposed();
            _list.RemoveAt(index);
            SetDirty();
        }

        #endregion

        #region Immutable functionallity

        public int Count => _list.Count;

        public bool IsReadOnly => IsDisposed || ((IList<T>)_list).IsReadOnly;

        public int IndexOf(T? item)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return _list.IndexOf(item);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Contains(T? item)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return _list.Contains(item);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        #endregion
    }
}