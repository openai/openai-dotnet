// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;
using OpenAI;

namespace OpenAI.VectorStores
{
    internal readonly partial struct InternalVectorStoreObjectObject : IEquatable<InternalVectorStoreObjectObject>
    {
        private readonly string _value;
        private const string VectorStoreValue = "vector_store";

        public InternalVectorStoreObjectObject(string value)
        {
            Argument.AssertNotNull(value, nameof(value));

            _value = value;
        }

        public static InternalVectorStoreObjectObject VectorStore { get; } = new InternalVectorStoreObjectObject(VectorStoreValue);

        public static bool operator ==(InternalVectorStoreObjectObject left, InternalVectorStoreObjectObject right) => left.Equals(right);

        public static bool operator !=(InternalVectorStoreObjectObject left, InternalVectorStoreObjectObject right) => !left.Equals(right);

        public static implicit operator InternalVectorStoreObjectObject(string value) => new InternalVectorStoreObjectObject(value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is InternalVectorStoreObjectObject other && Equals(other);

        public bool Equals(InternalVectorStoreObjectObject other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

        public override string ToString() => _value;
    }
}
