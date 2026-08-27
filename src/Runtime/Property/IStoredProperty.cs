// ReSharper disable once CheckNamespace

namespace Appegy.Storage
{
    /// <summary>
    ///     Provides typed read and write access to a single key in an <see cref="IBinaryStorage" />.
    /// </summary>
    /// <typeparam name="T">The type registered for the stored value.</typeparam>
    /// <remarks>
    ///     The property is a read-through and write-through view: it does not cache the value, and it
    ///     remains valid only while its underlying storage remains valid.
    /// </remarks>
    public interface IStoredProperty<T>
    {
        /// <summary>Gets the current value from storage or writes a new value to storage.</summary>
        T? Value { get; set; }
    }
}