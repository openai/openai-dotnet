using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemTruncatedGA")]
public partial class GARealtimeServerUpdateConversationItemTruncated
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioEndMs")]
    public TimeSpan AudioEndTime { get; }
}
