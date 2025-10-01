using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeResponseAudioContentPart")]
public partial class InternalRealtimeResponseAudioContentPart : ConversationContentPart
{
    [CodeGenMember("Transcript")]
    public string InternalTranscriptValue { get; }
}
