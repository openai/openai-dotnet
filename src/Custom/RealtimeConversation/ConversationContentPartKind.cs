using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeContentPartType")]
public readonly partial struct ConversationContentPartKind
{
    [CodeGenMember("Audio")]
    public static ConversationContentPartKind OutputAudio { get; } = new(OutputAudioValue);

    [CodeGenMember("Text")]
    public static ConversationContentPartKind OutputText { get; } = new(OutputTextValue);
}