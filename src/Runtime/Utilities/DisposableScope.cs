using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal class DisposableScope : IDisposable
    {
        private Action? _disposeCallback;
        private bool _disposed;

        public DisposableScope(Action disposeCallback)
        {
            _disposeCallback = disposeCallback;
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            Interlocked.Exchange(ref _disposeCallback, null)?.Invoke();
        }
    }
}