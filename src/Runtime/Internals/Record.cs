using System;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal abstract class Record
    {
        public abstract Type Type { get; }
        public abstract int TypeIndex { get; }
        public abstract object? Object { get; }

        public abstract IReactiveCollection? AsReactiveCollection();
    }

    internal class Record<T> : Record
    {
        private static readonly bool _valueCanBeReactiveCollection
            = typeof(IReactiveCollection).IsAssignableFrom(typeof(T));

        public Record(T value, int typeIndex)
        {
            Type = typeof(T);
            TypeIndex = typeIndex;
            Value = value;
        }

        public override Type Type { get; }
        public override int TypeIndex { get; }
        public override object? Object => Value;
        public T Value { get; set; }

        public override IReactiveCollection? AsReactiveCollection()
        {
            return _valueCanBeReactiveCollection ? Value as IReactiveCollection : null;
        }
    }
}