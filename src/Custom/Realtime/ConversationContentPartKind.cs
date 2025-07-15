using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeContentPartType")]
public readonly partial struct ConversationContentPartKind
{
    [CodeGenMember("Audio")]
    public static ConversationContentPartKind OutputAudio { get; } = new(AudioValue);

    [CodeGenMember("Text")]
    public static ConversationContentPartKind OutputText { get; } = new(TextValue);
}