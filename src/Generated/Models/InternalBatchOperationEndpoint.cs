// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;

namespace OpenAI.Batch
{
    internal readonly partial struct InternalBatchOperationEndpoint : IEquatable<InternalBatchOperationEndpoint>
    {
        private readonly string _value;

        public InternalBatchOperationEndpoint(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private const string V1ChatCompletionsValue = "/v1/chat/completions";
        private const string V1EmbeddingsValue = "/v1/embeddings";

        public static InternalBatchOperationEndpoint V1ChatCompletions { get; } = new InternalBatchOperationEndpoint(V1ChatCompletionsValue);
        public static InternalBatchOperationEndpoint V1Embeddings { get; } = new InternalBatchOperationEndpoint(V1EmbeddingsValue);
        public static bool operator ==(InternalBatchOperationEndpoint left, InternalBatchOperationEndpoint right) => left.Equals(right);
        public static bool operator !=(InternalBatchOperationEndpoint left, InternalBatchOperationEndpoint right) => !left.Equals(right);
        public static implicit operator InternalBatchOperationEndpoint(string value) => new InternalBatchOperationEndpoint(value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is InternalBatchOperationEndpoint other && Equals(other);
        public bool Equals(InternalBatchOperationEndpoint other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value;
    }
}
