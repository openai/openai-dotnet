using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseDoneCommand")]
public partial class ConversationResponseFinishedUpdate
{
    [CodeGenMember("Response")]
    internal readonly InternalRealtimeResponse _internalResponse;

    public string Id => _internalResponse?.Id;
    public ConversationStatus? Status => _internalResponse?.Status;

    // TODO
    internal InternalRealtimeResponseStatusDetails StatusDetails { get; }

    [CodeGenMember("Output")]
    public IReadOnlyList<ConversationItem> CreatedItems => _internalResponse?.Output ?? [];
}
