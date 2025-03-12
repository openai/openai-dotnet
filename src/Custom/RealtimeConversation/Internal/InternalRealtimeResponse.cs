using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeResponse")]
internal partial class InternalRealtimeResponse
{
    [CodeGenMember("Output")]
    public IReadOnlyList<ConversationItem> Output { get; }

    [CodeGenMember("Modalities")]
    internal IReadOnlyList<InternalRealtimeResponseModality> Modalities { get; }

    [CodeGenMember("Voice")]
    public ConversationVoice? Voice { get; }

    [CodeGenMember("OutputAudioFormat")]
    public ConversationAudioFormat? OutputAudioFormat { get; }
}
