using System;
using System.Collections.Generic;

namespace OpenAI.RealtimeConversation;

internal partial class InternalRealtimeNoTurnDetection : ConversationTurnDetectionOptions
{
    public InternalRealtimeNoTurnDetection()
        : this(ConversationTurnDetectionKind.Disabled, null)
    { }

    internal InternalRealtimeNoTurnDetection(ConversationTurnDetectionKind type, IDictionary<string, BinaryData> serializedAdditionalRawData)
        : base(type, serializedAdditionalRawData)
    { }
}
