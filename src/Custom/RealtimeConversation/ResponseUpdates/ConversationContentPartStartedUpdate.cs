using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseContentPartAddedCommand")]
public partial class ConversationContentPartStartedUpdate
{
    [CodeGenMember("Part")]
    private readonly ConversationContentPart _internalContentPart;

    public ConversationContentPartKind ContentKind => _internalContentPart switch
    {
        InternalRealtimeResponseTextContentPart => ConversationContentPartKind.InputText,
        InternalRealtimeResponseAudioContentPart => ConversationContentPartKind.InputAudio,
        _ => null,
    };

    public string Text => (_internalContentPart as InternalRealtimeResponseTextContentPart)?.Text;
    public string AudioTranscript => (_internalContentPart as InternalRealtimeResponseAudioContentPart)?.Transcript;
}
