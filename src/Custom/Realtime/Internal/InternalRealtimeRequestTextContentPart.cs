using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;
public partial class InternalRealtimeRequestTextContentPart : ConversationContentPart
{
    [CodeGenMember("Text")]
    public string InternalTextValue { get; set; }
}
