using System;

namespace Appegy.Storage
{
    public static class StorageExtensions
    {
        /// <summary>
        ///     Creates a nested storage that isolates keys using the specified prefix.
        /// </summary>
        public static IBinaryStorage CreateChild(this IBinaryStorage root, string prefix)
        {
            return new NestedBinaryStorage(root, prefix);
        }

        /// <summary>
        ///     Creates a typed, read/write property bound to a single storage key.
        /// </summary>
        /// <typeparam name="T">The registered, non-collection type of the stored value.</typeparam>
        /// <param name="storage">The storage that owns the key.</param>
        /// <param name="key">The key to bind to the property.</param>
        /// <param name="defaultValue">The value returned, and optionally stored, when the key is missing.</param>
        /// <param name="missingKeyBehavior">An optional override for the storage's missing-key behavior.</param>
        /// <param name="typeMismatchBehaviour">An optional override for the storage's type-mismatch behavior.</param>
        /// <returns>A property whose <see cref="IStoredProperty{T}.Value" /> accesses the specified key.</returns>
        /// <remarks>
        ///     Creating the property validates type support but does not create the key. Reading
        ///     <see cref="IStoredProperty{T}.Value" /> initializes a missing key only when the effective behavior is
        ///     <see cref="MissingKeyBehavior.InitializeWithDefaultValue" />. The property does not own or dispose
        ///     <paramref name="storage" />.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="storage" /> or <paramref name="key" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ObjectDisposedException">Thrown when <paramref name="storage" /> is disposed.</exception>
        /// <exception cref="IncorrectUsageOfCollectionException">
        ///     Thrown when <typeparamref name="T" /> is a collection type.
        /// </exception>
        /// <exception cref="UnregisteredTypeException">
        ///     Thrown when <typeparamref name="T" /> is not registered in <paramref name="storage" />.
        /// </exception>
        public static IStoredProperty<T> GetProperty<T>
        (
            this IBinaryStorage storage,
            string key,
            T? defaultValue = default(T?),
            MissingKeyBehavior? missingKeyBehavior = null,
            TypeMismatchBehaviour? typeMismatchBehaviour = null
        )
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!storage.Supports<T>())
                throw new UnregisteredTypeException(typeof(T));

            return new StoredProperty<T>(
                storage,
                key,
                defaultValue,
                missingKeyBehavior,
                typeMismatchBehaviour
            );
        }
    }
}