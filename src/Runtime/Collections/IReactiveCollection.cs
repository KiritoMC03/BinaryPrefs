using System;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal interface IReactiveCollection : IDisposable
    {
        event Action<IReactiveCollection> OnChanged;
    }
}