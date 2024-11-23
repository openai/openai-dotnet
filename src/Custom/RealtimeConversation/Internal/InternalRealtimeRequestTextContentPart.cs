using System;
using System.Collections.Generic;

namespace OpenAI.RealtimeConversation;
internal partial class InternalRealtimeRequestTextContentPart : ConversationContentPart
{
    [CodeGenMember("Text")]
    public string InternalTextValue { get; set; }
}
