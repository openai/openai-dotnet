using System;
using System.Collections.Generic;

namespace OpenAI.RealtimeConversation;

internal partial class InternalRealtimeResponseAudioContentPart : ConversationContentPart
{
    [CodeGenMember("Transcript")]
    public string InternalTranscriptValue { get; }
}
