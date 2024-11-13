using System;
using System.Collections.Generic;

namespace OpenAI.RealtimeConversation;
internal partial class InternalRealtimeRequestAudioContentPart : ConversationContentPart
{
    [CodeGenMember("Transcript")]
    public string InternalTranscriptValue { get; set; }
}
