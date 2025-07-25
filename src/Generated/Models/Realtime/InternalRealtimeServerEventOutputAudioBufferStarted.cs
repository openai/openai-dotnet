// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Realtime
{
    internal partial class InternalRealtimeServerEventOutputAudioBufferStarted : RealtimeUpdate
    {
        internal InternalRealtimeServerEventOutputAudioBufferStarted(string responseId) : base(RealtimeUpdateKind.OutputAudioBufferStarted)
        {
            ResponseId = responseId;
        }

        internal InternalRealtimeServerEventOutputAudioBufferStarted(RealtimeUpdateKind kind, string eventId, IDictionary<string, BinaryData> additionalBinaryDataProperties, string responseId) : base(kind, eventId, additionalBinaryDataProperties)
        {
            ResponseId = responseId;
        }

        public string ResponseId { get; }
    }
}
