using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeContentPart")]
public partial class ConversationContentPart
{
    [CodeGenMember("Type")]
    internal ConversationContentPartKind Type;

    public ConversationContentPartKind Kind => Type;

    public string TextValue =>
        (this as InternalRealtimeRequestTextContentPart)?.Text
        ?? (this as InternalRealtimeResponseTextContentPart)?.Text;

    public string AudioTranscriptValue =>
        (this as InternalRealtimeRequestAudioContentPart)?.Transcript
        ?? (this as InternalRealtimeResponseAudioContentPart)?.Transcript;

    public static ConversationContentPart FromInputText(string text)
        => new InternalRealtimeRequestTextContentPart(text);
    public static ConversationContentPart FromInputAudioTranscript(string transcript = null) => new InternalRealtimeRequestAudioContentPart()
    {
        Transcript = transcript,
    };
    public static ConversationContentPart FromOutputText(string text)
        => new InternalRealtimeResponseTextContentPart(text);
    public static ConversationContentPart FromOutputAudioTranscript(string transcript = null)
        => new InternalRealtimeResponseAudioContentPart(transcript);

    public static implicit operator ConversationContentPart(string text) => FromInputText(text);
}