// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses
{
    [Experimental("OPENAI001")]
    public partial class StreamingResponseCompletedUpdate : StreamingResponseUpdate
    {
        internal StreamingResponseCompletedUpdate(int sequenceNumber, OpenAIResponse response) : base(InternalResponseStreamEventType.ResponseCompleted, sequenceNumber)
        {
            Response = response;
        }

        internal StreamingResponseCompletedUpdate(InternalResponseStreamEventType kind, int sequenceNumber, IDictionary<string, BinaryData> additionalBinaryDataProperties, OpenAIResponse response) : base(kind, sequenceNumber, additionalBinaryDataProperties)
        {
            Response = response;
        }

        public OpenAIResponse Response { get; }
    }
}
