// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using OpenAI;

namespace OpenAI.Responses
{
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseReasoningSummaryVerbosity : IEquatable<ResponseReasoningSummaryVerbosity>
    {
        private readonly string _value;
        private const string AutoValue = "auto";
        private const string ConciseValue = "concise";
        private const string DetailedValue = "detailed";

        public ResponseReasoningSummaryVerbosity(string value)
        {
            Argument.AssertNotNull(value, nameof(value));

            _value = value;
        }

        public static ResponseReasoningSummaryVerbosity Auto { get; } = new ResponseReasoningSummaryVerbosity(AutoValue);

        public static ResponseReasoningSummaryVerbosity Concise { get; } = new ResponseReasoningSummaryVerbosity(ConciseValue);

        public static ResponseReasoningSummaryVerbosity Detailed { get; } = new ResponseReasoningSummaryVerbosity(DetailedValue);

        public static bool operator ==(ResponseReasoningSummaryVerbosity left, ResponseReasoningSummaryVerbosity right) => left.Equals(right);

        public static bool operator !=(ResponseReasoningSummaryVerbosity left, ResponseReasoningSummaryVerbosity right) => !left.Equals(right);

        public static implicit operator ResponseReasoningSummaryVerbosity(string value) => new ResponseReasoningSummaryVerbosity(value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is ResponseReasoningSummaryVerbosity other && Equals(other);

        public bool Equals(ResponseReasoningSummaryVerbosity other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

        public override string ToString() => _value;
    }
}
