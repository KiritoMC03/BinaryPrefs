using System;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal sealed class StoredProperty<T> : IStoredProperty<T>
    {
        private readonly T? _defaultValue;
        private readonly string _key;
        private readonly MissingKeyBehavior? _missingKeyBehavior;
        private readonly IBinaryStorage _storage;
        private readonly TypeMismatchBehaviour? _typeMismatchBehaviour;

        internal StoredProperty
        (
            IBinaryStorage storage,
            string key,
            T? defaultValue,
            MissingKeyBehavior? missingKeyBehavior,
            TypeMismatchBehaviour? typeMismatchBehaviour
        )
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _defaultValue = defaultValue;
            _missingKeyBehavior = missingKeyBehavior;
            _typeMismatchBehaviour = typeMismatchBehaviour;
        }

        public T? Value
        {
            get => _storage.Get(_key, _defaultValue, _missingKeyBehavior);
            set => _storage.Set(_key, value, _typeMismatchBehaviour);
        }
    }
}