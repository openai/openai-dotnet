// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Responses
{
    internal partial class InternalResponsesResponseStreamEventResponseFailed : StreamingResponseUpdate
    {
        internal InternalResponsesResponseStreamEventResponseFailed(OpenAIResponse response) : base(StreamingResponseUpdateKind.ResponseFailed)
        {
            Response = response;
        }

        internal InternalResponsesResponseStreamEventResponseFailed(StreamingResponseUpdateKind kind, IDictionary<string, BinaryData> additionalBinaryDataProperties, OpenAIResponse response) : base(kind, additionalBinaryDataProperties)
        {
            Response = response;
        }

        public OpenAIResponse Response { get; }
    }
}
