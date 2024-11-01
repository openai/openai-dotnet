using System;
using System.Collections.Generic;

namespace OpenAI.RealtimeConversation;

internal partial class InternalRealtimeResponseTextContentPart : ConversationContentPart
{
    [CodeGenMember("Text")]
    public string InternalTextValue { get; }
}
