using System;
using System.Collections.Generic;
using System.IO;
using Appegy.Storage.Serializers;
using FluentAssertions;
using NUnit.Framework;

namespace Appegy.Storage
{
    public class StoredPropertyTests : BaseStorageTests
    {
        [Test]
        public void WhenPropertyCreated_ThenKeyIsInitializedOnlyAfterValueIsRead()
        {
            using var storage = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(Int32Serializer.Shared)
                .Build();

            var property = storage.GetProperty("score", 42);

            storage.Has("score").Should().BeFalse();
            property.Value.Should().Be(42);
            storage.Has("score").Should().BeTrue();
            storage.Get<int>("score").Should().Be(42);
        }

        [Test]
        public void WhenMissingKeyBehaviorReturnsDefaultOnly_ThenReadingDoesNotCreateKey()
        {
            using var storage = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(Int32Serializer.Shared)
                .Build();

            var property = storage.GetProperty(
                "score",
                42,
                MissingKeyBehavior.ReturnDefaultValueOnly);

            property.Value.Should().Be(42);
            storage.Has("score").Should().BeFalse();
        }

        [Test]
        public void WhenPropertyAndStorageWriteSameKey_ThenBothObserveLatestValue()
        {
            using var storage = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(Int32Serializer.Shared)
                .Build();

            var property = storage.GetProperty("score", 0);

            property.Value = 10;
            storage.Get<int>("score").Should().Be(10);

            storage.Set("score", 20);
            property.Value.Should().Be(20);
        }

        [Test]
        public void WhenCustomTypeIsRegistered_ThenPropertySupportsIt()
        {
            using var storage = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(new StoredValueSerializer())
                .Build();

            var property = storage.GetProperty("custom", new StoredValue(3));

            property.Value = new StoredValue(7);

            property.Value.Should().Be(new StoredValue(7));
            storage.Get<StoredValue>("custom").Should().Be(new StoredValue(7));
        }

        [Test]
        public void WhenPropertyCreatedFromNestedStorage_ThenItUsesNestedKeyPrefix()
        {
            using var root = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(Int32Serializer.Shared)
                .Build();

            var nested = root.CreateChild("player");
            var property = nested.GetProperty("score", 0);

            property.Value = 15;

            root.Get<int>("__player->score").Should().Be(15);
            root.Has("score").Should().BeFalse();
        }

        [Test]
        public void WhenPropertyTypeIsNotRegistered_ThenCreationThrows()
        {
            using var storage = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(Int32Serializer.Shared)
                .Build();

            Action action = () => storage.GetProperty("custom", new StoredValue(1));

            action.Should().Throw<UnregisteredTypeException>();
        }

        [Test]
        public void WhenPropertyTypeIsCollection_ThenCreationThrowsWithCollectionGuidance()
        {
            using var storage = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(Int32Serializer.Shared)
                .SupportListsOf<int>()
                .Build();

            Action action = () => storage.GetProperty("values", new List<int>());

            action.Should().Throw<IncorrectUsageOfCollectionException>();
        }

        [Test]
        public void WhenPropertyOverridesTypeMismatchBehavior_ThenAssignmentReplacesStoredType()
        {
            using var storage = BinaryStorage.Construct(StoragePath)
                .AddTypeSerializer(Int32Serializer.Shared)
                .AddTypeSerializer(StringSerializer.Shared)
                .SetTypeMismatchBehaviour(TypeMismatchBehaviour.ThrowException)
                .Build();

            storage.Set("value", "old");
            var property = storage.GetProperty(
                "value",
                0,
                typeMismatchBehaviour: TypeMismatchBehaviour.OverrideValueAndType);

            property.Value = 12;

            storage.TypeOf("value").Should().Be(typeof(int));
            property.Value.Should().Be(12);
        }

        private readonly struct StoredValue : IEquatable<StoredValue>
        {
            public readonly int Number;

            public StoredValue(int number)
            {
                Number = number;
            }

            public bool Equals(StoredValue other)
            {
                return Number == other.Number;
            }

            public override bool Equals(object obj)
            {
                return obj is StoredValue other && Equals(other);
            }

            public override int GetHashCode()
            {
                return Number;
            }
        }

        private sealed class StoredValueSerializer : EquatableTypeSerializer<StoredValue>
        {
            public override void WriteTo(BinaryWriter writer, StoredValue value)
            {
                writer.Write(value.Number);
            }

            public override StoredValue ReadFrom(BinaryReader reader)
            {
                return new StoredValue(reader.ReadInt32());
            }
        }
    }
}
