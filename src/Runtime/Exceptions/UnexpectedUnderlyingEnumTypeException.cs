using System;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    public class UnexpectedUnderlyingEnumTypeException : Exception
    {
        public UnexpectedUnderlyingEnumTypeException
            (Type enumType, Type underlyingType) : base(
            $"Unexpected underlying type of enum {enumType.FullName} - {underlyingType.FullName}"
        )
        {
            EnumType = enumType;
            UnderlyingType = underlyingType;
        }

        public Type EnumType { get; }
        public Type UnderlyingType { get; }
    }
}