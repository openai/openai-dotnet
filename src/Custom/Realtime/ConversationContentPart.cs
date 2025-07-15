using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeContentPart")]
public partial class ConversationContentPart
{
    public string Text =>
        (this as InternalRealtimeRequestTextContentPart)?.InternalTextValue
        ?? (this as InternalRealtimeResponseTextContentPart)?.InternalTextValue;

    public string AudioTranscript =>
        (this as InternalRealtimeRequestAudioContentPart)?.InternalTranscriptValue
        ?? (this as InternalRealtimeResponseAudioContentPart)?.InternalTranscriptValue;

    public static ConversationContentPart CreateInputTextPart(string text)
        => new InternalRealtimeRequestTextContentPart(text);

    public static ConversationContentPart CreateInputAudioTranscriptPart(string transcript = null)
        => new InternalRealtimeRequestAudioContentPart()
        {
            InternalTranscriptValue = transcript,
        };

    public static ConversationContentPart CreateOutputTextPart(string text)
        => new InternalRealtimeResponseTextContentPart(text);

    public static ConversationContentPart CreateOutputAudioTranscriptPart(string transcript = null)
        => new InternalRealtimeResponseAudioContentPart(transcript);

    public static implicit operator ConversationContentPart(string text) => CreateInputTextPart(text);
}