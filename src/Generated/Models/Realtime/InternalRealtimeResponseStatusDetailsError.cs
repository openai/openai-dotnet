// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Realtime
{
    internal partial class InternalRealtimeResponseStatusDetailsError
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal InternalRealtimeResponseStatusDetailsError()
        {
        }

        internal InternalRealtimeResponseStatusDetailsError(string kind, string code, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            Kind = kind;
            Code = code;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Kind { get; }

        public string Code { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
