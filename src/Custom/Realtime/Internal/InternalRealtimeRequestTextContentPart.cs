using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;
internal partial class InternalRealtimeRequestTextContentPart : ConversationContentPart
{
    [CodeGenMember("Text")]
    public string InternalTextValue { get; set; }
}
