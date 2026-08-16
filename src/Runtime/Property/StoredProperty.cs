using System;
using JetBrains.Annotations;

namespace Appegy.Storage
{
    internal sealed class StoredProperty<T> : IStoredProperty<T>
    {
        [NotNull]
        private readonly IBinaryStorage _storage;
        [NotNull]
        private readonly string _key;
        [CanBeNull]
        private readonly T _defaultValue;
        [CanBeNull]
        private readonly MissingKeyBehavior? _missingKeyBehavior;
        [CanBeNull]
        private readonly TypeMismatchBehaviour? _typeMismatchBehaviour;

        [CanBeNull]
        public T Value
        {
            get => _storage.Get(_key, _defaultValue, _missingKeyBehavior);
            set => _storage.Set(_key, value, _typeMismatchBehaviour);
        }

        internal StoredProperty(
            [NotNull] IBinaryStorage storage,
            [NotNull] string key,
            [CanBeNull] T defaultValue,
            [CanBeNull] MissingKeyBehavior? missingKeyBehavior,
            [CanBeNull] TypeMismatchBehaviour? typeMismatchBehaviour)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _defaultValue = defaultValue;
            _missingKeyBehavior = missingKeyBehavior;
            _typeMismatchBehaviour = typeMismatchBehaviour;
        }
    }
}
