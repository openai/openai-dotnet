// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;
using OpenAI;

namespace OpenAI.Internal
{
    internal readonly partial struct InternalResponseFormatType : IEquatable<InternalResponseFormatType>
    {
        private readonly string _value;
        private const string TextValue = "text";
        private const string JsonObjectValue = "json_object";
        private const string JsonSchemaValue = "json_schema";

        public InternalResponseFormatType(string value)
        {
            Argument.AssertNotNull(value, nameof(value));

            _value = value;
        }

        internal static InternalResponseFormatType Text { get; } = new InternalResponseFormatType(TextValue);

        internal static InternalResponseFormatType JsonObject { get; } = new InternalResponseFormatType(JsonObjectValue);

        internal static InternalResponseFormatType JsonSchema { get; } = new InternalResponseFormatType(JsonSchemaValue);

        public static bool operator ==(InternalResponseFormatType left, InternalResponseFormatType right) => left.Equals(right);

        public static bool operator !=(InternalResponseFormatType left, InternalResponseFormatType right) => !left.Equals(right);

        public static implicit operator InternalResponseFormatType(string value) => new InternalResponseFormatType(value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is InternalResponseFormatType other && Equals(other);

        public bool Equals(InternalResponseFormatType other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

        public override string ToString() => _value;
    }
}
